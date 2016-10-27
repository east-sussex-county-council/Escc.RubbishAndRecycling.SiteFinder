using System.Configuration;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Escc.RubbishAndRecycling.SiteFinder.Website.EastSussexGovUkWebForms), "PostStart")]

namespace Escc.RubbishAndRecycling.SiteFinder.Website {

    /// <summary>
    /// Register the virtual path provider which makes available the embedded views from Escc.EastSussexGovUK.Mvc
    /// </summary>
    public static class EastSussexGovUkWebForms
	{
  		/// <summary>
		/// Wire up the provider at the end of the application startup process 
		/// </summary>
	    public static void PostStart() 
		{
            // This check avoids creating a dependence on Escc.EastSussexGovUK.WebForms for the Escc.RubbishAndRecycling.SiteFinder NuGet package
            if (ConfigurationManager.AppSettings["RecyclingSiteFinderLoadWebFormsTemplate"] == "true")
  		    {
  		        LoadWebFormsTemplate();
  		    }
		}

        private static void LoadWebFormsTemplate()
        {
            System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedResourceVirtualPathProvider.Vpp(typeof (Escc.EastSussexGovUK.WebForms.BaseMasterPage).Assembly));
        }
	}
}