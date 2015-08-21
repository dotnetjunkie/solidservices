namespace WebApiService.CompositionRoot
{
    using System.Diagnostics;
    using BusinessLayer;

    public sealed class DebugLogger : ILogger
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }
    }
}