using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using Escc.Net;
using EsccWebTeam.Data.Web;
using Exceptionless.Json;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// Gets recycling site data from an Umbraco installation using an API defined in the Escc.CustomerFocusTemplates.Website project
    /// </summary>
    public class UmbracoRecyclingSiteDataSource : IRecyclingSiteDataSource
    {
        private readonly string _wasteType;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoRecyclingSiteDataSource"/> class.
        /// </summary>
        /// <param name="wasteType">Type of the waste.</param>
        public UmbracoRecyclingSiteDataSource(string wasteType)
        {
            _wasteType = wasteType;
        }

        public void AddRecyclingSites(DataTable table)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["RecyclingSiteDataUrl"]))
            {
                throw new ConfigurationErrorsException("appSettings/RecyclingSiteDataUrl setting not found");
            }

            var url = ConfigurationManager.AppSettings["RecyclingSiteDataUrl"];
            if (!String.IsNullOrEmpty(_wasteType))
            {
                url += "&acceptsWaste=" + HttpUtility.UrlEncode(_wasteType);
            }
            var absoluteUrl = Iri.MakeAbsolute(new Uri(url, UriKind.RelativeOrAbsolute), new Uri(Uri.UriSchemeHttps + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.Url.AbsolutePath));
            var client = new HttpRequestClient(new ConfigurationProxyProvider());
            var request = client.CreateRequest(absoluteUrl);
#if DEBUG
            // Turn off SSL check in debug mode as it will always fail against a self-signed certificate used for development
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
#endif
            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var json = reader.ReadToEnd();

                    var locations = JsonConvert.DeserializeObject<List<LocationApiResult>>(json);

                    foreach (var location in locations)
                    {
                        var row = table.NewRow();
                        row["Title"] = location.Name;
                        row["URL"] = location.Url;
                        row["Latitude"] = location.Latitude;
                        row["Longitude"] = location.Longitude;
                        table.Rows.Add(row);
                    }
                }
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class LocationApiResult
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Url { get; set; }
    }
}


