using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AspNetCore_Middleware
{
    public class MyMiddlewareClass
    {
        private RequestDelegate next;

        public MyMiddlewareClass(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await context.Response.WriteAsync(
                "My middleware class is handling the request" + Environment.NewLine);
            await this.next.Invoke(context);
            await context.Response.WriteAsync(
                "My middleware class has completed handling the request" + Environment.NewLine);
        }
    }
}
