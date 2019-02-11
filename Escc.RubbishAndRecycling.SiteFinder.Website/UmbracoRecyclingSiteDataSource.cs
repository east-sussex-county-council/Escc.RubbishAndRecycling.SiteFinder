using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Escc.Net;
using Newtonsoft.Json;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// Gets recycling site data from an Umbraco installation using an API defined in the Escc.CustomerFocusTemplates.Website project
    /// </summary>
    public class UmbracoRecyclingSiteDataSource : IRecyclingSiteDataSource
    {
        private Uri _recyclingSiteDataUrl;
        private readonly string _wasteType;
        private readonly IProxyProvider _proxyProvider;
        private static HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoRecyclingSiteDataSource"/> class.
        /// </summary>
        /// <param name="recyclingSiteDataUrl">The URL to connect to for recycling site data</param>
        /// <param name="wasteType">Type of the waste.</param>
        /// <param name="proxyProvider">A method of getting the proxy for connecting to the data source</param>
        public UmbracoRecyclingSiteDataSource(Uri recyclingSiteDataUrl, string wasteType, IProxyProvider proxyProvider)
        {
            _recyclingSiteDataUrl = recyclingSiteDataUrl ?? throw new ArgumentNullException(nameof(recyclingSiteDataUrl));
            _wasteType = wasteType;
            _proxyProvider = proxyProvider;
        }

        /// <summary>
        /// Gets recycling site data and adds it to the data table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public async Task AddRecyclingSites(DataTable table)
        {
            if (table == null) throw new ArgumentNullException("table");
            
            if (!String.IsNullOrEmpty(_wasteType))
            {
                _recyclingSiteDataUrl = new Uri(_recyclingSiteDataUrl.ToString() +  "&acceptsWaste=" + HttpUtility.UrlEncode(_wasteType));
            }

            if (_httpClient == null)
            {
                _httpClient = new HttpClient(new HttpClientHandler()
                {
                    Proxy = _proxyProvider?.CreateProxy()
                });
            }
            var json = await _httpClient.GetStringAsync(_recyclingSiteDataUrl);

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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class LocationApiResult
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Url { get; set; }
    }
}


