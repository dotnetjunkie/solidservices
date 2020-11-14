using WebCoreService.Code;

namespace WebCoreService
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    // NOTE: Here are two example urls for queries:
    // * http://localhost:49228/api/queries/GetUnshippedOrdersForCurrentCustomer?Paging.PageIndex=3&Paging.PageSize=10
    // * http://localhost:49228/api/queries/GetOrderById?OrderId=97fc6660-283d-44b6-b170-7db0c2e2afae
    public class Startup
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),

#if DEBUG
            Formatting = Formatting.Indented,
#endif
        };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            // We need to—at least—call AddMvcCore(), because it registers 11 implementations of the
            // IActionResultExecutor<TResult> interface. Those are required by the HttpContextExtensions
            // method. Ideally, the use of MVC should not be required at all, which can considerably lower
            // the deployment footprint, but we're not there yet. Feedback is welcome.
            services
                .AddMvcCore()
                .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor accessor)
        {
            var bootstrapper = new Bootstrapper(accessor);

            // Map routes to the middleware for query handling and command handling
            app.Map("/api/queries",
                b => UseMiddleware(b,
                    new QueryHandlerMiddleware(info => bootstrapper.GetQueryHandler(info.QueryType), JsonSettings)));

            app.Map("/api/commands",
                b => UseMiddleware(b, new CommandHandlerMiddleware(bootstrapper.GetCommandHandler, JsonSettings)));

            bootstrapper.Verify();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }

        private static void UseMiddleware(IApplicationBuilder app, IMiddleware middleware) =>
            app.Use((c, next) => middleware.InvokeAsync(c, _ => next()));
    }
}