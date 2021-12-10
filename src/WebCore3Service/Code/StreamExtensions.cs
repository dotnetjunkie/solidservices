namespace WebCoreService.Code
{
    using System.IO;
    public static class StreamExtensions
    {
        public static string ReadToEnd(this Stream stream)
        {
            string result;
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
    }
}