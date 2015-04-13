using System;
using System.Configuration;
using System.Web;
using System.Web.UI.HtmlControls;
using EsccWebTeam.Data.Web;
using EsccWebTeam.EastSussexGovUK;

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
                var siteContext = new EastSussexGovUKContext();
                var redirectToUrl = new Uri(ConfigurationManager.AppSettings["RecyclingSiteFinderBaseUrl"] + "?postcode=" + HttpUtility.UrlEncode(Request.Form[this.postcode.UniqueID]) + "&type=" + Request.Form[this.wasteTypes.UniqueID]);
                if (siteContext.RequestUrl.ToString() != redirectToUrl.ToString())
                {
                    Http.Status303SeeOther(redirectToUrl);
                }
            }
        }

        /// <summary>
        /// Populates the waste type drop down and inserts an 'all types' item at index 0.
        /// Reads data from a simple xml file whose path is specified in web.congig and places this in cache as a dataset.
        /// Uses cache dependency to update the dataset if the file changes on disk.
        /// </summary>
        private void PopulateWasteTypes()
        {

            var wasteTypesData = new UmbracoWasteTypesDataSource();

            this.wasteTypes.DataSource = wasteTypesData.LoadWasteTypes();
            this.wasteTypes.DataBind();
            this.wasteTypes.Items.Insert(0, "Anything");
        }
    }
}
