using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UploaderWebSite.Services;

namespace UploaderWebSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

          
            routes.Add("File-route", new Route("upload", new UploadFileRouteHandler()));

            routes.MapRoute(
                "Cancel", // Route name
                "upload/cancel", // URL with parameters
                null
            );
        }
    }
}