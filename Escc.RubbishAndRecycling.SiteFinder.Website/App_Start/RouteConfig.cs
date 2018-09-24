using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Escc.RubbishAndRecycling.SiteFinder.Website.App_Start
{
    /// <summary>
    /// Set up the URLs for the application
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Preserve old URLs from when this was a WebForms app
            routes.MapRoute(
                name: "WebForms",
                url: "{controller}.aspx",
                defaults: new { controller = "Default", action = "Index" }
            );

            // Home page
            routes.MapRoute(
                name: "Default",
                url: String.Empty,
                defaults: new { controller = "Default", action = "Index" }
            );
        }
    }
}