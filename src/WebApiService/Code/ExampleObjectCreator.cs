using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebApiService.Code
{
    public static class ExampleObjectCreator
    {
        private readonly static Dictionary<Type, object> sampleObjects = new Dictionary<Type, object>();

        public static object GetSampleInstance(Type type)
        {
            lock (sampleObjects)
            {
                object sampleObject;

                if (!sampleObjects.TryGetValue(type, out sampleObject))
                {
                    sampleObjects[type] = sampleObject = CreateSampleInstance(type);
                }

                return sampleObject;
            }
        }

        private static object CreateSampleInstance(Type type)
        {
            if (type == typeof(DateTime))
            {
                return DateTime.Now;
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            if (type == typeof(string))
            {
                return "sample string";
            }

            if (type == typeof(Uri))
            {
                return new Uri("http://webapihelppage.com");
            }

            if (type.IsArray || (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
            {
                return CreateSampleCollection(type) ?? CreateComplexObject(type);
            }

            return CreateComplexObject(type);
        }

        private static object CreateComplexObject(Type type)
        {
            var constructor = (
                from ctor in type.GetConstructors()
                orderby ctor.GetParameters().Length
                select ctor)
                .First();

            var constructorArguments =
                from parameter in constructor.GetParameters()
                select GetSampleInstance(parameter.ParameterType);

            object instance = Activator.CreateInstance(type, constructorArguments.ToArray());

            var properties =
                from property in type.GetProperties()
                where property.CanRead && property.CanWrite && property.GetSetMethod() != null
                select property;

            foreach (var property in properties)
            {
                try
                {
                    property.SetValue(instance, GetSampleInstance(property.PropertyType));
                }
                catch
                {
                    // On error resume next ;-)
                }
            }

            return instance;
        }

        private static object CreateSampleCollection(Type type)
        {
            if (type.IsArray)
            {
                return CreateSampleArray(type);
            }

            try
            {
                if (typeof(IList).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    return CreateList(type);
                }

                Type def = type.GetGenericTypeDefinition();

                if (typeof(IList<>).IsAssignableFrom(def) || typeof(ICollection<>).IsAssignableFrom(def))
                {
                    return CreateList(typeof(List<>).MakeGenericType(GetElementType(type)));
                }
            }
            catch
            {
                // We return null for weird collections
            }

            return null;
        }

        private static object CreateList(Type type)
        {
            var list = (IList)Activator.CreateInstance(type);
            list.Add(CreateSampleInstance(GetElementType(type)));
            return list;
        }

        private static object[] CreateSampleArray(Type type)
        {
            Type elementType = GetElementType(type);

            return new[] { CreateSampleInstance(elementType) };
        }

        private static Type GetElementType(Type type) => (
            from interfaceType in type.GetInterfaces()
            where interfaceType.IsGenericType
            where interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            select interfaceType.GetGenericArguments()[0])
            .First();
    }
}