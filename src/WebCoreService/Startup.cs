using WebCoreService.Code;

namespace WebCoreService
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.ViewComponents;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SimpleInjector;
    using SimpleInjector.Integration.AspNetCore.Mvc;
    using SimpleInjector.Lifestyles;

    public class Startup
    {
        private readonly Container container = new Container();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            IntegrateSimpleInjector(services);
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            this.container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(this.container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(this.container));

            services.EnableSimpleInjectorCrossWiring(this.container);
            services.UseSimpleInjectorAspNetRequestScoping(this.container);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Bootstrapper.Bootstrap(this.container, app);

            // Add custom middleware
            app.Map("/api/queries", builder => builder.UseMiddleware<QueryHandlerMiddleware>(this.container));
            app.Map("/api/commands", builder => builder.UseMiddleware<CommandHandlerMiddleware>(this.container));

            this.container.Verify();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
