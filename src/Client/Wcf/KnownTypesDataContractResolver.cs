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
            this.knownTypes = types.Distinct().ToDictionary(GetName);
        }

        public override Type ResolveName(
            string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            try
            {
                Type type;

                return this.knownTypes.TryGetValue(typeName, out type)
                    ? type
                    : knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unable to resolve type {typeName}. {ex.InnerException} " +
                    "If the given type name is postfixed with a weird base64 encoded value, it means that " +
                    "the type is a generic type. WCF postfixes the name with a hash based on the " +
                    "namespaces of the generic type arguments. To fix this, mark the class with the " +
                    "DataContractAttribute to force a specific name. Example:" +
                    "[DataContract(Name = nameof(YourType<T>) + \"Of{0}\")]. And don't forget to mark the type's " +
                    "properties with the DataMemberAttribute.", ex);
            }            
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