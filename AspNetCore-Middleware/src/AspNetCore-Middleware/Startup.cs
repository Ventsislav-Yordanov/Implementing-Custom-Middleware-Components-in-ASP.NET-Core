using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

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

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Hello from coponent 1 ◕‿◕" + Environment.NewLine);
                await next.Invoke();
                await context.Response.WriteAsync("Hello from coponent 1 again ◕‿◕" + Environment.NewLine);
            });

            app.UseMyMiddleware();

            app.Map("/mymapbranch", (appbuilder) =>
            {
                appbuilder.Use(async (context, next) =>
                {
                    await next.Invoke();
                });

                appbuilder.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Greetings from my Map Branch ◕‿↼" + Environment.NewLine);
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
