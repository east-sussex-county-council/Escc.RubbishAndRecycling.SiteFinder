using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Escc.EastSussexGovUK;
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
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly ICacheStrategy<List<string>> _cacheStrategy;
        private static HttpClient _httpClient;

        /// <summary>
        /// Creates a new instance of <see cref="UmbracoWasteTypesDataSource"/>
        /// </summary>
        /// <param name="wasteTypesDataUrl"></param>
        /// <param name="httpClientProvider"></param>
        /// <param name="cacheStrategy">A method of caching the list of waste types</param>
        public UmbracoWasteTypesDataSource(Uri wasteTypesDataUrl, IHttpClientProvider httpClientProvider, ICacheStrategy<List<string>> cacheStrategy)
        {
            _wasteTypesDataUrl = wasteTypesDataUrl ?? throw new ArgumentNullException(nameof(wasteTypesDataUrl));
            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
            _cacheStrategy = cacheStrategy;
        }

        /// <summary>
        /// Gets the waste types from an Umbraco API
        /// </summary>
        /// <returns></returns>
        public async Task<IList<string>> LoadWasteTypes()
        {
            if (_cacheStrategy != null)
            {
                var cachedWasteTypes = _cacheStrategy.ReadFromCache(this.ToString());
                if (cachedWasteTypes != null) return cachedWasteTypes;
            }

            if (_httpClient == null)
            {
                _httpClient = _httpClientProvider.GetHttpClient();
            }
            var json = await _httpClient.GetStringAsync(_wasteTypesDataUrl).ConfigureAwait(false);
            var wasteTypes = JsonConvert.DeserializeObject<List<string>>(json);
            wasteTypes.Insert(0, "Anything");

            if (_cacheStrategy != null)
            {
                _cacheStrategy.AddToCache(this.ToString(), wasteTypes);
            }

            return wasteTypes;
        }

    }
}


