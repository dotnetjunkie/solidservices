using WebCoreService.Code;

namespace WebCoreService
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using SimpleInjector;

    public class Startup
    {
        private readonly Container container = new Container();

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),

#if DEBUG
            Formatting = Formatting.Indented,
#endif
        };

        public Startup(IConfiguration configuration)
        {
            container.Options.ResolveUnregisteredConcreteTypes = false;

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // We need to—at least—call AddMvcCore, because it registers 11 implementations of the
            // IActionResultExecutor<TResult> interface. Those are required by the HttpContextExtensions
            // method.
            services
                .AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSimpleInjector(this.container, options =>
            {
                options.AddAspNetCore();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSimpleInjector(this.container);

            Bootstrapper.Bootstrap(this.container);

            // Map routes to the middleware for query handling and command handling
            app.Map("/api/queries",
                b => UseMiddleware(b, new QueryHandlerMiddleware(this.container, JsonSettings)));

            app.Map("/api/commands",
                b => UseMiddleware(b, new CommandHandlerMiddleware(this.container, JsonSettings)));

            this.container.Verify();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        private static void UseMiddleware(IApplicationBuilder app, IMiddleware middleware) =>
            app.Use((c, next) => middleware.InvokeAsync(c, _ => next()));
    }
}