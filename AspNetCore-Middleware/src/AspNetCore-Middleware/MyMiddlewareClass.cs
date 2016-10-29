using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AspNetCore_Middleware
{
    public class MyMiddlewareClass
    {
        private RequestDelegate next;
        private IOptions<MyMiddlewareOptions> options;

        public MyMiddlewareClass(RequestDelegate next, IOptions<MyMiddlewareOptions> options)
        {
            this.next = next;
            this.options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Items["message"] = "The weather is great today!";
            await context.Response.WriteAsync(this.options.Value.OptionOne + Environment.NewLine);
            await this.next.Invoke(context);
            await context.Response.WriteAsync(
                "My middleware class has completed handling the request" + Environment.NewLine);
        }
    }
}
