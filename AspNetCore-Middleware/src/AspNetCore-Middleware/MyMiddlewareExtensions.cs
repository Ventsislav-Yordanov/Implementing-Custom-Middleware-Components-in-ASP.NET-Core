using Microsoft.AspNetCore.Builder;

namespace AspNetCore_Middleware
{
    public static class MyMiddlewareExtensions
    {

        public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MyMiddlewareClass>();
        }
    }
}
