using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApi_ElGas.Utils;

namespace WebApi_ElGas
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CorreoUtil.SmtpServer = System.Configuration.ConfigurationManager.AppSettings["SmtpServer"];
            CorreoUtil.Port = System.Configuration.ConfigurationManager.AppSettings["SmtpPort"];
            var Ssl = true;
            if (System.Configuration.ConfigurationManager.AppSettings["EnableSsl"] == "True")
            {
                Ssl = true;
            }
            else
            {
                Ssl = false;
            }

            CorreoUtil.EnableSsl = Ssl;
            CorreoUtil.UserName = System.Configuration.ConfigurationManager.AppSettings["Usuario"];
            CorreoUtil.Password = System.Configuration.ConfigurationManager.AppSettings["Contrasena"];
        }
    }
}
