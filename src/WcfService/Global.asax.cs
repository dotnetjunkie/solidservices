namespace WcfService
{
    using System;
    
    using WcfService.CompositionRoot;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Bootstrapper.Bootstrap();
        }
    }
}