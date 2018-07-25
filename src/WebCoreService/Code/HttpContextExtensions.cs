namespace WebCoreService.Code
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    //https://github.com/aspnet/Mvc/issues/7238#issuecomment-357391426
    public static class HttpContextExtensions
    {
        private static readonly RouteData emptyRouteData = new RouteData();

        private static readonly ActionDescriptor emptyActionDescriptor = new ActionDescriptor();
        //TODO - need to ensure that any IActionResult works here or need to change to only allow ObjectResult
        public static Task WriteResultAsync<TResult>(this HttpContext context, TResult result)
            where TResult : IActionResult
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var executor = context.RequestServices.GetService<IActionResultExecutor<TResult>>();

            if (executor == null)
                throw new InvalidOperationException(
                    $"No result executor for '{typeof(TResult).FullName}' has been registered.");

            var routeData = context.GetRouteData() ?? emptyRouteData;

            var actionContext = new ActionContext(context, routeData, emptyActionDescriptor);

            return executor.ExecuteAsync(actionContext, result);
        }
    }
}