namespace WebApiService.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Web.Http.Controllers;

    /// <summary>
    /// A <see cref="HttpControllerDescriptor"/> with extra information.
    /// </summary>
    public class CommandControllerDescriptor
    {
        public CommandControllerDescriptor(HttpControllerDescriptor controllerDescriptor, string action,
            Type messageType, string method)
        {
            this.ControllerDescriptor = controllerDescriptor;
            this.Action = action;
            this.MessageType = messageType;
            this.Method = method;
        }

        public HttpControllerDescriptor ControllerDescriptor { get; private set; }

        public string Action { get; private set; }

        public Type MessageType { get; set; }

        public string Method { get; private set; }

        public IEnumerable<Parameter> GetWebApiParameters()
        {
            return this.GetWebApiParameters(this.MessageType);
        }

        private IEnumerable<Parameter> GetWebApiParameters(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                Type propertyType = property.PropertyType;

                if (propertyType.IsPrimitive || propertyType == typeof(string) ||
                    (propertyType.Namespace.StartsWith("System") && propertyType.IsValueType))
                {
                    yield return new Parameter(property.Name, property.PropertyType);
                }
                else
                {
                    foreach (var subProperty in this.GetWebApiParameters(property.PropertyType))
                    {
                        yield return new Parameter(property.Name + "." + subProperty.Name, subProperty.Type);
                    }
                }
            }
        }

        public class Parameter
        {
            public Parameter(string name, Type type)
            {
                this.Name = name;
                this.Type = type;
            }

            public string Name { get; private set; }

            public Type Type { get; private set; }
        }
    }
}