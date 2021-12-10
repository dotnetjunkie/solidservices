using Contract;
using SimpleInjector;

namespace WebCoreService;

// This class is named "Commands" to allow Swagger to group query handler routes.
public sealed record Queries(Container Container)
{
    public async Task<TResult> InvokeAsync<TQuery, TResult>(HttpContext context, TQuery query)
        where TQuery : IQuery<TResult>
    {
        var handler = Container.GetInstance<IQueryHandler<TQuery, TResult>>();

        try
        {
            TResult result = handler.Handle(query);
            return result;
        }
        catch (Exception exception)
        {
            var response = WebApiErrorResponseBuilder.CreateErrorResponseOrNull(exception);

            if (response != null)
            {
                await response.ExecuteAsync(context);

                return default!;
            }
            else
            {
                throw;
            }
        }
    }
}