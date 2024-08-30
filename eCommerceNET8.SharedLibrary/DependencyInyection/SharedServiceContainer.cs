using eCommerceNET8.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerceNET8.SharedLibrary.DependencyInyection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedSevices<TContext>(this IServiceCollection services,IConfiguration config, string fileName) where TContext : DbContext
        {
            //Add Generic Database context
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                config.GetConnectionString( "eCommerceConnection"), sqlserverOption =>
                sqlserverOption.EnableRetryOnFailure()));
            //configure serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}[{Level:u3}]{message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day).CreateLogger();

            //Add JWT authetication Scheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            //use global Exception
            app.UseMiddleware<GlobalException>();
            //Register middleware to block all outsiders API calls
            app.UseMiddleware<LisenToOnlyApiGateway>();
            return app;
        }
    }
}
