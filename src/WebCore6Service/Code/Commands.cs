namespace WebCoreService;

using BusinessLayer;
using SimpleInjector;

// This class is named "Commands" to allow Swagger to group command handler routes.
public sealed record Commands(Container Container)
{
    public Task<IResult> InvokeAsync<TCommand>(TCommand command)
    {
        var handler = Container.GetInstance<ICommandHandler<TCommand>>();

        try
        {
            handler.Handle(command);
            return Task.FromResult(Results.Ok());
        }
        catch (Exception exception)
        {
            var response = WebApiErrorResponseBuilder.CreateErrorResponseOrNull(exception);

            if (response != null)
            {
                return Task.FromResult(response);
            }
            else
            {
                throw;
            }
        }
    }
}
