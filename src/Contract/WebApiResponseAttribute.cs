namespace Contract
{
    using System;
    using System.Net;

    // This attribute is not used by the WCF service, but will can be used by a ASP.NET Web API service.
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class WebApiResponseAttribute : Attribute
    {
        public WebApiResponseAttribute(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public Type Location { get; set; }

        public static HttpStatusCode GetHttpStatusCode(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(WebApiResponseAttribute), false);

            return attributes.Length == 0 ? HttpStatusCode.OK : ((WebApiResponseAttribute)attributes[0]).StatusCode;
        }

        public static Type GetLocation(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(WebApiResponseAttribute), false);

            return attributes.Length == 0 ? null : ((WebApiResponseAttribute)attributes[0]).Location;
        }
    }
}