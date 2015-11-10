﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Lunggo.Framework.BrowserDetection;
using Lunggo.Framework.Config;

namespace Lunggo.CustomerWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AppInitializer.Init();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        
        /*
        void Session_Start(object sender, EventArgs e)
        {
            // Redirect mobile users to the mobile home page
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Browser.IsMobileDevice)
            {
                var configManager = ConfigManager.GetInstance();
                var mobileUrl = configManager.GetConfigValue("general", "mobileUrl");
                var host = httpRequest.Url.Host;
                var path = httpRequest.Url.PathAndQuery;
                var userAgent = httpRequest.UserAgent;
                var browserDetectionService = BrowserDetectionService.GetInstance();
                var isSmartphone = browserDetectionService.IsRequestFromSmartphone(userAgent);
                var isOnMobilePage = host == mobileUrl && isSmartphone;
                if (!isOnMobilePage)
                {
                    string redirectTo = "http://" + mobileUrl + path;

                    // Could also add special logic to redirect from certain 
                    // recognized pages to the mobile equivalents of those 
                    // pages (where they exist). For example,
                    // if (HttpContext.Current.Handler is UserRegistration)
                    //     redirectTo = "~/Mobile/Register.aspx";

                    HttpContext.Current.Response.Redirect(redirectTo);
                }
            }
            
        }
        */
    }
}
