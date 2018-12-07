using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Escc.Geo;
using Escc.Net;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    public class DefaultController : Controller
    {
        private string _postCode;
        private string _wasteType;

        // GET: Default
        public ActionResult Index()
        {
            var model = new RecyclingViewModel();

            // Ensure there's one version of this URL so that the data is consistent in Google Analytics
            if (Path.GetFileName(Request.RawUrl).ToUpperInvariant() == "DEFAULT.ASPX")
            {
                return new RedirectResult(new Uri(Request.Url, Url.Content("~/")).ToString(), true);
            }

            // Feed options from another page
            if (!String.IsNullOrEmpty(Request.QueryString["type"]))
            {
                _postCode = Request.QueryString["postcode"];
                _wasteType = Request.QueryString["type"];
                _wasteType = _wasteType.Replace("â€“", "–"); // Using Response.AddHeader() with an ndash in the URL does a bad conversion of the Unicode character, so convert it back

                // Check for old wording and redirect rather than error
                if (_wasteType == "All waste types")
                {
                    var thisPage = Url.Content("~/");
                    var redirectTo = new Uri(thisPage + "?postcode=" + _postCode + "&type=Anything", UriKind.Relative);
                    return new RedirectResult(new Uri(new Uri(Uri.UriSchemeHttps + "://" + Request.Url.Authority + Request.Url.AbsolutePath), redirectTo).ToString(), true);
                }

                var wasteTypes = new UmbracoWasteTypesDataSource();
                if (_wasteType != "Anything" && !IsValidRecyclableItemType(_wasteType, wasteTypes))
                {
                    return new HttpStatusCodeResult(400);
                }
                else
                {
                    model.RecyclingSites = GetAndBindData();
                }
            }

            return View(model);
        }

        private static bool IsValidRecyclableItemType(string type, IWasteTypesDataSource wasteTypes)
        {
            var possibleTypes = wasteTypes.LoadWasteTypes();
            return possibleTypes.Contains(type);
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
        private DataView GetAndBindData()
        {
            // need to convert miles to metres 
            Int32 rad = (int)ConvertMilesToMetres(Convert.ToDouble(60));

            // call the appropriate method which returns a dataset
            DataSet ds = GetSiteData();
            DataView dv = null;
            if (_postCode != null)
            {
                ds = GetNearestRecyclingSitesRadialFromCms(ds, rad, _postCode);

                if (ds != null)
                {
                    // get a default view on the dataset which we can then sort 
                    dv = ds.Tables[0].DefaultView;

                    // sort by distance
                    dv.Sort = "Miles ASC";
                }
            }
            else
            {
                // Searching without a postcode is simply a way to link to all sites for search engines to index them.
                // Users can see this view, but are not expected to use it.

                // get a default view on the dataset which we can then sort 
                var dataToTrim = ds.Clone();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    dataToTrim.Tables[0].ImportRow(row);
                }
                dv = dataToTrim.Tables[0].DefaultView;

                // sort by name
                dv.Sort = "Title ASC";
            }

            return dv;
        }

        /// <summary>
        /// Returns recycling sites in East Sussex for a given postcode and distance. Returns results within a given radial area rather than a square.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="dist">A distance, in metres, to build a 'circular' reference area from. Must not exceed 100000m.</param>
        /// <param name="nearPostcode">A valid postcode.</param>
        /// <returns>
        /// An ADO.net dataset.
        /// </returns>
        public DataSet GetNearestRecyclingSitesRadialFromCms(DataSet dataSet, Double dist, string nearPostcode)
        {
            DataSet results;
            var postcodeLookup = new LocateApiPostcodeLookup(new Uri(ConfigurationManager.AppSettings["LocateApiAuthorityUrl"]), ConfigurationManager.AppSettings["LocateApiToken"], new ConfigurationProxyProvider());
            var centreOfPostcode = postcodeLookup.CoordinatesAtCentreOfPostcode(nearPostcode);
            if (centreOfPostcode == null) return null;
            var distanceCalculator = new DistanceCalculator();

            results = dataSet.Clone();
            Double radius = dist;

            // need to loop through each row pull out the easting and northing for the site and run it through a method which checks if the
            // site is within the chosen radius
            foreach (DataRow dr in dataSet.Tables[0].Rows)
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
        /// <seealso cref="DataSetFromCms">
        /// The method which calls the CM Server web service.
        /// </seealso>
        /// <returns>An ADO.net DataSet.</returns>
        private DataSet GetSiteData()
        {
            var cacheKey = "wastesitedata";
            if (_postCode == null) cacheKey += "-no-postcode";
            if (_wasteType != "Anything") cacheKey += Regex.Replace(_wasteType, "[^A-Za-z]", String.Empty);

            DataSet dsCms = HttpContext.Cache.Get(cacheKey) as DataSet;
            if (dsCms == null)
            {
                dsCms = DataSetFromCms();
                HttpContext.Cache.Insert(cacheKey, dsCms, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return dsCms;
        }

        /// <summary>
        /// Extracts placeholder content and uses this to create a dataset.
        /// </summary>
        /// <returns>DataSet of all household waste and recycling centres.</returns>
        public DataSet DataSetFromCms()
        {
            using (var ds = RecyclingSiteDataFormat.CreateDataSet())
            {
                // apply filtering if a specific waste type has been selected by the user
                new UmbracoRecyclingSiteDataSource(_wasteType != "Anything" ? _wasteType : String.Empty).AddRecyclingSites(ds.Tables[0]);
                ds.AcceptChanges();
                return ds;
            }
        }
    }
}