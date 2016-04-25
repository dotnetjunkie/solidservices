namespace Client.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;

    // source: https://msdn.microsoft.com/en-us/library/dd807519%28v=vs.110%29.aspx
    public sealed class KnownTypesDataContractResolver : DataContractResolver
    {
        private readonly Dictionary<string, Type> knownTypes;

        public KnownTypesDataContractResolver(IEnumerable<Type> types)
        {
            this.knownTypes = (
                from type in types.Distinct()
                group type by type.Name into g
                select g.First())
                .ToDictionary(GetName);
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType,
            DataContractResolver knownTypeResolver)
        {
            Type type;

            return this.knownTypes.TryGetValue(typeName, out type)
                ? type
                : knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
        }

        [DebuggerStepThrough]
        public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver,
            out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if (!knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace))
            {
                typeName = new XmlDictionaryString(XmlDictionary.Empty, type.Name, 0);
                typeNamespace = new XmlDictionaryString(XmlDictionary.Empty, type.Namespace, 0);
            }

            return true;
        }

        private static string GetName(Type type) =>
            type.IsArray
                ? "ArrayOf" + GetName(type.GetElementType())
                : type.IsGenericType ? GetGenericName(type) : type.Name;

        private static string GetGenericName(Type type)
        {
            Type typeDef = type.GetGenericTypeDefinition();
            string name = typeDef.Name.Substring(0, typeDef.Name.IndexOf('`'));
            return name + "Of" + string.Join(string.Empty, type.GetGenericArguments().Select(GetName));
        }
    }
}