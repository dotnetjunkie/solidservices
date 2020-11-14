namespace WcfService
{
    using System;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Bootstrapper.Instance.Verify();
        }
    }
}