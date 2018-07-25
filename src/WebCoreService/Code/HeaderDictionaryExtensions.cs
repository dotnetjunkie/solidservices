namespace WebCoreService.Code
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    public static class HeaderDictionaryExtensions
    {
        public static string GetValueOrNull(this IHeaderDictionary headers, string key)
        {
            if (!headers.TryGetValue(key, out StringValues value))
                return null;
            return value[0];
        }
    }
}
