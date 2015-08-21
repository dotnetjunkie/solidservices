namespace WebApiService.Code
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.ModelBinding;
    using Contract;

    // Generic ApiController to handle queries using the IQueryProcessor.
    public class QueryController<TQuery, TResult> : ApiController where TQuery : IQuery<TResult>
    {
        private static readonly HttpStatusCode SuccessStatusCodeForThisQueryType;

        private readonly IQueryProcessor processor;

        static QueryController()
        {
            SuccessStatusCodeForThisQueryType = WebApiResponseAttribute.GetHttpStatusCode(typeof(TQuery));
        }

        public QueryController(IQueryProcessor processor)
        {
            this.processor = processor;
        }

        // Note: without [ModelBinder] or [FromUri], the query object won't get deserialized.
        public HttpResponseMessage Execute([ModelBinder]TQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            TResult result;

            try
            {
                result = this.processor.Execute(query);
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

            return this.Request.CreateResponse<TResult>(SuccessStatusCodeForThisQueryType, result);
        }
    }
}