using eCommerceNET8.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace eCommerceNET8.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Declare variable
            string message = "sorry internal server error ocurred. Kindly try again";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);
                // check if Ecxecption is too many request 429 status code.
                if(context.Response.StatusCode== StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many request made.";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context , title , message , statusCode);
                }
                // If Response UnAuthorized// 401 status code.
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not authorized to access.";
                   
                    await ModifyHeader(context, title, message, statusCode);
                }
                // If Response is Forbidden// 403 status code.
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    message = "You are not allowed/required to access.";
                    statusCode = StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                //Log Original Exception/File,Debugger,Console 
                LogException.LogExceptions(ex);
                //chech if Ecxecption is Timeout
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of Time";
                    message = "Request timeout....try again.";
                    statusCode = StatusCodes.Status408RequestTimeout;
                   
                }
                ///if none of the ecxeptions || Exception caught then do the default
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            //display scary-free message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
                {
                Detail = message,
                Status = statusCode,
                Title = title
            }),CancellationToken.None);
        }
    }
}
