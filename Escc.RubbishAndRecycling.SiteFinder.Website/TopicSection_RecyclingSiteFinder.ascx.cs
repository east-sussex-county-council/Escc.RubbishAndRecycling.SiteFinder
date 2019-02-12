using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.HtmlControls;
using Escc.EastSussexGovUK;
using Escc.Net.Configuration;
using Escc.Web;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    ///	User control encapsulating Household waste and recycling point search controls and functions.
    /// </summary>
    public partial class RecyclingSiteFinder : System.Web.UI.UserControl
    {
        protected HtmlGenericControl h2;

        /// <summary>
        /// Gets or sets whether to display a title for this control.
        /// </summary>
        /// <value>
        ///   <c>true</c> to display title; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayTitle { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.h2.Visible = DisplayTitle;

            // Use HasKeys test instead of IsPostback or a Click event, so that it works in MVC
            if (!Request.Form.HasKeys())
            {
                //populate the waste type drop down.
                PopulateWasteTypes();

                // Feed options from another page
                if (!String.IsNullOrEmpty(Request.QueryString["postcode"]))
                {
                    this.postcode.Text = Request.QueryString["postcode"];
                }
                if (!String.IsNullOrEmpty(Request.QueryString["type"]))
                {
                    var item = this.wasteTypes.Items.FindByText(Request.QueryString["type"]);
                    if (item != null)
                    {
                        this.wasteTypes.ClearSelection();
                        item.Selected = true;
                    }
                }
            }
            else
            {
                // Check for setting
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["RecyclingSiteFinderBaseUrl"]))
                {
                    throw new ConfigurationErrorsException("<appSettings><add key=\"RecyclingSiteFinderBaseUrl\" /></appSettings> is missing");
                }

                // Redirect to URL which can be bookmarked, and tested in Google Analytics
                // Use Request.Form to access postback data so that it works from MVC
                var redirectToUrl = new Uri(ConfigurationManager.AppSettings["RecyclingSiteFinderBaseUrl"] + "?postcode=" + HttpUtility.UrlEncode(Request.Form[this.postcode.UniqueID]) + "&type=" + Request.Form[this.wasteTypes.UniqueID], UriKind.RelativeOrAbsolute);
                if (!IsSameQueryAgain(redirectToUrl))
                {
                    new HttpStatus().SeeOther(new Uri(new Uri(Uri.UriSchemeHttps + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.Url.AbsolutePath), redirectToUrl));
                }
            }
        }

        private bool IsSameQueryAgain(Uri redirectToUrl)
        {
            if (redirectToUrl == null) throw new ArgumentNullException("redirectToUrl");

            if (redirectToUrl.IsAbsoluteUri)
            {
                return Request.Url.ToString() == redirectToUrl.ToString();
            }
            else
            {
                return Request.Url.PathAndQuery == redirectToUrl.ToString();                
            }
        }

        /// <summary>
        /// Populates the waste type drop down and inserts an 'all types' item at index 0.
        /// Reads data from a simple xml file whose path is specified in web.congig and places this in cache as a dataset.
        /// Uses cache dependency to update the dataset if the file changes on disk.
        /// </summary>
        private void PopulateWasteTypes()
        {
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["WasteTypesDataUrl"]))
            {
                throw new ConfigurationErrorsException("appSettings/WasteTypesDataUrl setting not found");
            }

            var url = ConfigurationManager.AppSettings["WasteTypesDataUrl"];
            var absoluteUrl = new Uri(new Uri(Uri.UriSchemeHttps + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.Url.AbsolutePath), new Uri(url, UriKind.RelativeOrAbsolute));

            var wasteTypesData = new UmbracoWasteTypesDataSource(absoluteUrl, new ConfigurationProxyProvider(), new ApplicationCacheStrategy<List<string>>(TimeSpan.FromDays(1)));

            this.wasteTypes.DataSource = wasteTypesData.LoadWasteTypes().Result;
            this.wasteTypes.DataBind();
        }
    }
}
