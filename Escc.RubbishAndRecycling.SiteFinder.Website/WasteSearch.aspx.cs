
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.XPath;
using Escc.Geo;
using EsccWebTeam.EastSussexGovUK;
using Escc.Exceptions.Soap;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// Search results for waste and recycling sites
    /// </summary>
    public partial class WasteSearch : System.Web.UI.Page
    {
        private SoapExceptionWrapper _helper;
        private string _postCode;
        private string _wasteType;
        private bool _error400 = true;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                // Feed options from another page
                if (!String.IsNullOrEmpty(Request.QueryString["postcode"]) && !String.IsNullOrEmpty(Request.QueryString["type"]))
                {
                    try
                    {
                        _postCode = Request.QueryString["postcode"];
                        _wasteType = Request.QueryString["type"];

                        var wasteTypes = new UmbracoWasteTypesDataSource();
                        if (_wasteType != "Anything" && !IsValidRecyclableItemType(_wasteType, wasteTypes))
                        {
                            EastSussexGovUKContext.HttpStatus400BadRequest(this.container);
                            _error400 = true;
                            return;
                        }

                        GetAndBindData();
                    }
                    catch (SoapException ex)
                    {

                        _helper = new SoapExceptionWrapper(ex);

                        if (_helper.Message.Contains("The postcode entered could not be found.") ||
                            _helper.Message.Contains("The postcode entered appears to be incorrect."))
                        {
                            litError.InnerHtml = "The postcode was not found in East Sussex. Please check the postcode and try again.";
                        }
                        else
                        {
                            litError.InnerHtml = _helper.Message + " ";
                            litError.InnerHtml += _helper.Description;
                        }

                        litError.InnerHtml = FormatException(litError.InnerHtml);
                        litError.Visible = true;

                        // hide paging controls and repeater if an error occurs
                        TogglePaging(false);
                    }
                }
                else
                {
                    // hide paging controls first time round
                    TogglePaging(false);

                    // check whether we are currently paging through results
                    if (_postCode != null & _wasteType != null)
                    {
                        try
                        {
                            GetAndBindData();
                        }
                        catch (SoapException ex)
                        {
                            _helper = new SoapExceptionWrapper(ex);

                            if (_helper.Message.Contains("The postcode entered could not be found.") ||
                                _helper.Message.Contains("The postcode entered appears to be incorrect."))
                            {
                                litError.InnerHtml = "The postcode was not found in East Sussex. Please check the postcode and try again.";
                            }
                            else
                            {
                                litError.InnerHtml = _helper.Message + " ";
                                litError.InnerHtml += _helper.Description;
                            }

                            litError.InnerHtml = FormatException(litError.InnerHtml);
                            litError.Visible = true;

                            // hide paging controls and repeater if an error occurs
                            TogglePaging(false);
                        }
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_error400)
            {
                this.related.Visible = false;
            }
            base.OnPreRender(e);
        }

        private static bool IsValidRecyclableItemType(string type, IWasteTypesDataSource wasteTypes)
        {
            var possibleTypes = wasteTypes.LoadWasteTypes();
            return possibleTypes.Contains(type);
        }

        /// <summary>
        /// Creates a bullet point error message
        /// </summary>
        /// <param name="message">string</param>
        /// <returns>string</returns>
        private static string FormatException(string message)
        {
            string startTags = "<div class=\"errorSummary\"><ul class=\"validationSummary\"><li>";
            string endTags = "</li></ul></div>";
            message = startTags + message + endTags;
            return message;
        }

        /// <summary>
        /// Simple function for converting miles to metres.
        /// </summary>
        /// <param name="miles">A double.</param>
        /// <returns>Miles converted to metres as a double.</returns>
        private static Double ConvertMilesToMetres(Double miles)
        {
            return (miles * 1.60934) * 1000;
        }

        /// <summary>
        /// Instantiates and uses web services to bind the repeater control with a list of nearest waste sites.
        /// Called from Page_Load and uses the values from the postcode textbox and the waste type and radial distance drop downs.
        /// The list is sorted in order of ascending distance and filtered on waste type if a type has been selected by the user.
        /// </summary>
        private void GetAndBindData()
        {
            //clear any error messages
            litError.InnerHtml = string.Empty;

            // need to convert miles to metres 
            Int32 rad = (int)ConvertMilesToMetres(Convert.ToDouble(60));

            // call the appropriate method which returns a dataset
            DataSet ds = GetNearestRecyclingSitesRadialFromCms(rad, _postCode);
            // get a default view on the dataset which we can then sort 
            DataView dv = ds.Tables[0].DefaultView;
            
            // sort by distance
            dv.Sort = "Miles ASC";

            // set up paging	
            paging.TrimRows(dv);

            if (paging.TotalResults > 0)
            {
                // bind data
                rptResults.DataSource = dv;
                rptResults.DataBind();

                // write paging navigation
                TogglePaging(true);
            }
            else
            {
                TogglePaging(false);
                litError.InnerHtml = FormatException("No sites were found matching your criteria.");
                litError.Visible = true;
            }
        }

        /// <summary>
        /// Sets paging visibility
        /// </summary>
        /// <param name="togVal">boolean. true = on, false = off.</param>
        private void TogglePaging(bool togVal)
        {
            if (!togVal)
            {
                pagingTop.Visible = false;
                pagingBottom.Visible = false;
                rptResults.Visible = false;
            }
            else
            {
                pagingTop.Visible = true;
                pagingBottom.Visible = true;
                rptResults.Visible = true;
            }
        }


        #region Read site data from CMS
        /// <summary>
        /// Returns recycling sites in East Sussex for a given postcode and distance. Returns results within a given radial area rather than a square. 
        /// </summary>
        /// <param name="dist">A distance, in metres, to build a 'circular' reference area from. Must not exceed 100000m.</param>	
        /// <param name="nearPostcode">A valid postcode.</param>
        /// <returns>An ADO.net dataset.</returns>		
        public DataSet GetNearestRecyclingSitesRadialFromCms(Double dist, string nearPostcode)
        {
            DataSet dsCms = GetSiteData();
            DataSet results;
            var postcodeLookup = new PostcodeLookupWebService();
            var centreOfPostcode = postcodeLookup.CoordinatesAtCentreOfPostcode(nearPostcode);
            var distanceCalculator = new DistanceCalculator();

            results = dsCms.Clone();
            Double radius = dist;

            // need to loop through each row pull out the easting and northing for the site and run it through a method which checks if the
            // site is within the chosen radius
            foreach (DataRow dr in dsCms.Tables[0].Rows)
            {
                // missing live data will likely be null or empty string from the cms placeholder?
                if (!String.IsNullOrWhiteSpace(dr["Latitude"].ToString()) || !String.IsNullOrWhiteSpace(dr["Longitude"].ToString()))
                {
                    var locationToCheck = new LatitudeLongitude(Convert.ToDouble(dr["Latitude"], CultureInfo.InvariantCulture), Convert.ToDouble(dr["Longitude"], CultureInfo.InvariantCulture));

                    var howFarAway = distanceCalculator.DistanceBetweenTwoPoints(centreOfPostcode, locationToCheck);

                    if (howFarAway <= radius)
                    {
                        results.Tables[0].ImportRow(dr);
                    }
                }                   
            }
            return GenerateDistances(results, centreOfPostcode, distanceCalculator);
        }

        /// <summary>
        /// Adds distances to a dataset of sites retrieved from Microsoft CMS.
        /// </summary>
        /// <param name="ds">A dataset containing sites and eastings and northings for each site.</param>
        /// <param name="centreOfPostcode">The centre of the user's postcode.</param>
        /// <param name="distanceCalculator">The distance calculator.</param>
        /// <returns>
        /// An ADO.net DataSet.
        /// </returns>
        public static DataSet GenerateDistances(DataSet ds, LatitudeLongitude centreOfPostcode, DistanceCalculator distanceCalculator)
        {
            if (ds == null) throw new ArgumentNullException("ds");
            if (distanceCalculator == null) throw new ArgumentNullException("distanceCalculator");

            using (DataColumn dcK = new DataColumn("Kilometres", Type.GetType("System.Double")))
            {
                using (DataColumn dcM = new DataColumn("Miles", Type.GetType("System.Double")))
                {
                    ds.Tables[0].Columns.Add(dcK);
                    ds.Tables[0].Columns.Add(dcM);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var latLongInDataRow = new LatitudeLongitude(Convert.ToDouble(dr["Latitude"].ToString(), CultureInfo.InvariantCulture), Convert.ToDouble(dr["Longitude"].ToString(), CultureInfo.InvariantCulture));
                        var kilometres = distanceCalculator.DistanceBetweenTwoPoints(centreOfPostcode, latLongInDataRow) / 1000;
                        dr["Kilometres"] = kilometres;
                        dr["Miles"] = Math.Round((kilometres * 0.6214), 2);
                    }
                    ds.AcceptChanges();
                    return ds;
                }
            }
        }

        /// <summary>
        /// Gets a DataSet of household waste and recycling centre info from Application or calls GetDataSetFromCMS() if no cached version exists.
        /// </summary>
        /// <seealso cref="GetDataSetFromCMS()">
        /// The method which calls the CM Server web service.
        /// </seealso>
        /// <returns>An ADO.net DataSet.</returns>
        private DataSet GetSiteData()
        {
            var cacheKey = "wastesitedata";
            if (_wasteType != "Anything") cacheKey += Regex.Replace(_wasteType, "[^A-Za-z]", String.Empty);

            DataSet dsCms = Cache.Get(cacheKey) as DataSet;
            if (dsCms == null)
            {
                dsCms = GetDataSetFromCMS();
                Cache.Insert(cacheKey, dsCms, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return dsCms;
        }

        /// <summary>
        /// Extracts placeholder content and uses this to create a dataset.
        /// </summary>
        /// <returns>DataSet of all household waste and recycling centres.</returns>
        public DataSet GetDataSetFromCMS()
        {
            using (var ds = RecyclingSiteDataFormat.CreateDataSet())
            {
                // apply filtering if a specific waste type has been selected by the user
                new UmbracoRecyclingSiteDataSource(_wasteType != "Anything" ? _wasteType : String.Empty).AddRecyclingSites(ds.Tables[0]);
                ds.AcceptChanges();
                return ds;
            }
        }
        #endregion

    }
}
