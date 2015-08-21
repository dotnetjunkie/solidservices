namespace WebApiService.Code
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.ModelBinding;
    using Contract;

    // Generic ApiController to handle commands using the ICommandHandler<TCommand>.
    public class CommandController<TCommand> : ApiController
    {
        private static readonly HttpStatusCode SuccessStatusCodeForThisCommandType;

        private readonly ICommandHandler<TCommand> commandHandler;

        static CommandController()
        {
            SuccessStatusCodeForThisCommandType = WebApiResponseAttribute.GetHttpStatusCode(typeof(TCommand));
        }

        public CommandController(ICommandHandler<TCommand> commandHandler)
        {
            this.commandHandler = commandHandler;
        }

        public HttpResponseMessage Execute([FromBody]TCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            try
            {
                this.commandHandler.Handle(command);
            }
            catch (Exception ex)
            {
                var response = WebApiErrorResponseBuilder.CreateErrorResponse(ex, this.Request);

                if (response != null)
                {
                    return response;
                }

                throw;
            }

            return this.Request.CreateResponse<TCommand>(SuccessStatusCodeForThisCommandType, command);
        }
    }
}