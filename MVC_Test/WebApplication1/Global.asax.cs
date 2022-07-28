using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            UnityConfig.RegisterComponents();
        }

        void Application_BeginRequest(Object source, EventArgs e)
        {
            HttpApplication app = (HttpApplication)source;
            HttpContext context = app.Context;
            Application["ApplicationPath"] = FirstRequestInitialisation.Initialise(context);

        }

        class FirstRequestInitialisation
        {
            private static string host = null;
            private static Object s_lock = new Object();
            public static string Initialise(HttpContext context)
            {
                if (string.IsNullOrEmpty(host))
                {
                    lock (s_lock)
                    {
                        if (string.IsNullOrEmpty(host))
                        {
                            Uri uri = EnvironmentSpecificRequestUrl(new HttpRequestWrapper(HttpContext.Current.Request));
#if DEBUG
                            host = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
#else
                            host = System.Configuration.ConfigurationManager.AppSettings["SitePath"];
#endif
                        }
                    }
                }
                return host;
            }
        }

        static Uri EnvironmentSpecificRequestUrl(HttpRequestBase request)
        {

            UriBuilder hostUrl = new UriBuilder();
            string hostHeader = request.Headers["Host"];

            if (hostHeader.Contains(":"))
            {
                hostUrl.Host = hostHeader.Split(':')[0];
                hostUrl.Port = Convert.ToInt32(hostHeader.Split(':')[1]);
            }
            else
            {
                hostUrl.Host = hostHeader;
                hostUrl.Port = -1;
            }

            Uri url = request.Url;
            UriBuilder uriBuilder = new UriBuilder(url);

            if (String.Equals(hostUrl.Host, "localhost", StringComparison.OrdinalIgnoreCase) || hostUrl.Host == "127.0.0.1")
            {
                // Do nothing
                // When we're running the application from localhost (e.g. debugging from Visual Studio), we'll keep everything as it is.
                // We're not using request.IsLocal because it returns true as long as the request sender and receiver are in same machine.
                // What we want is to only ignore the requests to 'localhost' or the loopback IP '127.0.0.1'.
                return uriBuilder.Uri;
            }

            // When the application is run behind a load-balancer (or forward proxy), request.IsSecureConnection returns 'true' or 'false'
            // based on the request from the load-balancer to the web server (e.g. IIS) and not the actual request to the load-balancer.
            // The same is also applied to request.Url.Scheme (or uriBuilder.Scheme, as in our case).
            bool isSecureConnection = String.Equals(request.Headers["X-Forwarded-Proto"], "https", StringComparison.OrdinalIgnoreCase);

            if (isSecureConnection)
            {
                uriBuilder.Port = hostUrl.Port == -1 ? 443 : hostUrl.Port;
                uriBuilder.Scheme = "https";
            }
            else
            {
                uriBuilder.Port = hostUrl.Port == -1 ? 80 : hostUrl.Port;
                uriBuilder.Scheme = "http";
            }

            uriBuilder.Host = hostUrl.Host;

            return uriBuilder.Uri;
        }
    }
}
