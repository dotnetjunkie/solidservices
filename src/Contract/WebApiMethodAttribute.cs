namespace Contract
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class WebApiMethodAttribute : Attribute
    {
        internal WebApiMethodAttribute(string method)
        {
            this.Method = method;
        }

        public string Method { get; private set; }

        public static string GetMethod(Type type)
        {
            var attributes = 
                (WebApiMethodAttribute[])type.GetCustomAttributes(typeof(WebApiMethodAttribute), false);

            if (attributes.Length == 0)
            {
                return null;
            }

            return attributes[0].Method;
        }
    }
}