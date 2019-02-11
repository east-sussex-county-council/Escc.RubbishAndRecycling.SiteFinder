using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Escc.Net;
using Newtonsoft.Json;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// Gets waste types data from an Umbraco installation using an API defined in the Escc.EastSussexGovUK.Umbraco.Web project
    /// </summary>
    public class UmbracoWasteTypesDataSource : IWasteTypesDataSource
    {
        private readonly Uri _wasteTypesDataUrl;
        private readonly IProxyProvider _proxyProvider;
        private static HttpClient _httpClient;

        /// <summary>
        /// Creates a new instance of <see cref="UmbracoWasteTypesDataSource"/>
        /// </summary>
        /// <param name="wasteTypesDataUrl"></param>
        /// <param name="proxyProvider"></param>
        public UmbracoWasteTypesDataSource(Uri wasteTypesDataUrl, IProxyProvider proxyProvider)
        {
            _wasteTypesDataUrl = wasteTypesDataUrl ?? throw new ArgumentNullException(nameof(wasteTypesDataUrl));
            _proxyProvider = proxyProvider;
        }

        /// <summary>
        /// Gets the waste types from an Umbraco API
        /// </summary>
        /// <returns></returns>
        public async Task<IList<string>> LoadWasteTypes()
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient(new HttpClientHandler()

                {
                    Proxy = _proxyProvider?.CreateProxy()
                });
            }
            var json = await _httpClient.GetStringAsync(_wasteTypesDataUrl).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<string>>(json);
        }

    }
}


