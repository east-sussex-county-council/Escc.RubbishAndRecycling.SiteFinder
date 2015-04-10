﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using EsccWebTeam.Data.Web;
using EsccWebTeam.Data.Xml;
using Exceptionless.Json;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// Gets waste types data from an Umbraco installation using an API defined in the Escc.CustomerFocusTemplates.Website project
    /// </summary>
    public class UmbracoWasteTypesDataSource : IWasteTypesDataSource
    {
        public IList<string> LoadWasteTypes()
        {
            if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["WasteTypesDataUrl"]))
            {
                throw new ConfigurationErrorsException("appSettings/WasteTypesDataUrl setting not found");
            }

            var url = ConfigurationManager.AppSettings["WasteTypesDataUrl"];
            var absoluteUrl = Iri.MakeAbsolute(new Uri(url));
            var request = XmlHttpRequest.Create(absoluteUrl);
#if DEBUG
            // Turn off SSL check in debug mode as it will always fail against a self-signed certificate used for development
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
#endif
            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var json = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<List<string>>(json);
                }
            }
        }

    }
}


