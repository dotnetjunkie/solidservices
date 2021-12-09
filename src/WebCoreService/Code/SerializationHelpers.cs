﻿namespace WebCoreService.Code
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public static class SerializationHelpers
    {
        // NOTE: arrays and dictionary formats are not supported. e.g. this won't work:
        // ?values[0]=a&values[1]=b&x[A]=1&x[B]=5
        public static string ConvertQueryStringToJson(string query)
        {
            var collection = HttpUtility.ParseQueryString(query);
            var dictionary = collection.AllKeys.ToDictionary(key => key, key => collection[key]);
            return ConvertDictionaryToJson(dictionary);
        }

        private static string ConvertDictionaryToJson(Dictionary<string, string> dictionary)
        {
            var propertyNames =
                from key in dictionary.Keys
                let index = key.IndexOf(value: '.')
                select index < 0 ? key : key.Substring(0, index);

            var data =
                from propertyName in propertyNames.Distinct()
                let json = dictionary.ContainsKey(propertyName)
                    ? HttpUtility.JavaScriptStringEncode(dictionary[propertyName], addDoubleQuotes: true)
                    : ConvertDictionaryToJson(FilterByPropertyName(dictionary, propertyName))
                select HttpUtility.JavaScriptStringEncode(propertyName, addDoubleQuotes: true) + ": " + json;

            return "{ " + string.Join(", ", data) + " }";
        }

        private static Dictionary<string, string> FilterByPropertyName(Dictionary<string, string> dictionary,
            string propertyName)
        {
            string prefix = propertyName + ".";
            return dictionary.Keys
                .Where(key => key.StartsWith(prefix))
                .ToDictionary(key => key.Substring(prefix.Length), key => dictionary[key]);
        }
    }
}