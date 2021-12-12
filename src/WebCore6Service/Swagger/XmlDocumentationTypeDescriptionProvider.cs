﻿namespace WebCoreService;

using System.Globalization;
using System.Xml.XPath;

// NOTE: The code in this file is copy-pasted from the default Web API Visual Studio 2013 template.
/// <summary>
/// Allows getting type descriptions based on .NET XML documentation files that are generated by the
/// C# or VB compiler.
/// </summary>
public sealed class XmlDocumentationTypeDescriptionProvider
{
    private const string TypeExpression = "/doc/members/member[@name='T:{0}']";

    private XPathNavigator _documentNavigator;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDocumentationTypeDescriptionProvider"/> class.
    /// </summary>
    /// <param name="documentPath">The physical path to XML document.</param>
    public XmlDocumentationTypeDescriptionProvider(string documentPath)
    {
        if (documentPath is null) throw new ArgumentNullException(nameof(documentPath));

        _documentNavigator = new XPathDocument(documentPath).CreateNavigator();
    }

    /// <summary>Gets the type's description or null when there is no description for the given type.</summary>
    /// <param name="type">The type.</param>
    /// <returns>The description of the requested type or null.</returns>
    public string? GetDescription(Type type)
    {
        XPathNavigator typeNode = GetTypeNode(type);
        return GetTagValue(typeNode, "summary");
    }

    private XPathNavigator GetTypeNode(Type type)
    {
        string controllerTypeName = GetTypeName(type);
        string selectExpression = String.Format(CultureInfo.InvariantCulture, TypeExpression, controllerTypeName);
        return _documentNavigator.SelectSingleNode(selectExpression)!;
    }

    private static string? GetTagValue(XPathNavigator parentNode, string tagName)
    {
        if (parentNode != null)
        {
            XPathNavigator? node = parentNode.SelectSingleNode(tagName);
            if (node != null)
            {
                return node.Value.Trim();
            }
        }

        return null;
    }

    private static string GetTypeName(Type type)
    {
        string name = type.FullName!;
        if (type.IsGenericType)
        {
            // Format the generic type name to something like: Generic{System.Int32,System.String}
            Type genericType = type.GetGenericTypeDefinition();
            Type[] genericArguments = type.GetGenericArguments();
            string genericTypeName = genericType.FullName!;

            // Trim the generic parameter counts from the name
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
            string[] argumentTypeNames = genericArguments.Select(t => GetTypeName(t)).ToArray();
            name = String.Format(CultureInfo.InvariantCulture, "{0}{{{1}}}", genericTypeName, String.Join(",", argumentTypeNames));
        }
        if (type.IsNested)
        {
            // Changing the nested type name from OuterType+InnerType to OuterType.InnerType to match the XML documentation syntax.
            name = name.Replace("+", ".");
        }

        return name;
    }
}