using Microsoft.AspNetCore.Http;

namespace eCommerceNET8.SharedLibrary.Middleware
{
    public class LisenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Extract specific header from the request
            var signedHeader = context.Request.Headers["Api.Gateway"];
            // Null means, the request is not coming from the Api Gateway//53 service
            if(signedHeader.FirstOrDefault()is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry , service is unavible");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
