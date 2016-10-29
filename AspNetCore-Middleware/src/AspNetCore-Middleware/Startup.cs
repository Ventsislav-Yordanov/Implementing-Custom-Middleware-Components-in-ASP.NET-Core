using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore_Middleware
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true);

            this.Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddCors();

            services.AddRouting();

            services.AddDistributedMemoryCache();
            services.AddSession();

            var myOptions = this.Configuration.GetSection("MyMiddlewareOptionsSection");
            services.Configure<MyMiddlewareOptions>(o => o.OptionOne = myOptions["OptionOne"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseSession();

            var routeBuilder = new RouteBuilder(app);

            routeBuilder.MapGet("greeting/{name}", appBuilder =>
            {
                appBuilder.Run(async (context) =>
                {
                    var name = context.GetRouteValue("name");
                    await context.Response.WriteAsync("Hey there, " + name);
                });
            });

            app.UseRouter(routeBuilder.Build());

            app.UseCors((builder) =>
            {
                builder.WithOrigins("http://localhost:6671/");
            });

            app.Use(async (context, next) =>
            {
                context.Session.SetString("my message", "you have completed setting a string in session");
                await context.Response.WriteAsync("Hello from coponent 1" + Environment.NewLine);
                await next.Invoke();
                await context.Response.WriteAsync("Hello from coponent 1 again" + Environment.NewLine);
                var myMessage = context.Session.GetString("my message");
                await context.Response.WriteAsync("Session message: " + myMessage + Environment.NewLine);
            });

            app.UseMyMiddleware();

            app.Map("/mymapbranch", (appBuilder) =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    await next.Invoke();
                });

                appBuilder.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Greetings from my Map Branch" + Environment.NewLine);
                });
            });

            app.MapWhen(context => context.Request.Query.ContainsKey("querybranch"), (appBuilder) =>
            {
                appBuilder.Run(async (context) =>
                {
                    await context.Response.WriteAsync("You have arrived at your MapWhen branch" + Environment.NewLine);
                });
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World! " + context.Items["message"] + Environment.NewLine);
            });
        }
    }
}
