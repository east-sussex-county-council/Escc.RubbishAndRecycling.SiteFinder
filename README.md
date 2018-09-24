Escc.RubbishAndRecycling.SiteFinder.Website
===========================================

Search facility for waste and recycling sites in East Sussex. 

This tool relies on two external tools: 

* an implementation of the GOV.UK [locate-api](https://github.com/alphagov/locate-api), which can look up a postcode and return a latitude and longitude for the centre point.

	This requires two entries in the `appSettings` section of `web.config`:

		<appSettings>
			<add key="LocateApiToken" value="12345"/>
		    <add key="LocateApiAuthorityUrl" value="https://hostname/locate/authority?postcode={0}"/>
		</appSettings>

* an API, defined in the `Escc.EastSussexGovUK.Umbraco` project, which provides recycling site data as JSON.

	This requires the URL to be set in the `appSettings` section of `web.config`:
	
		<appSettings>
    		<add key="RecyclingSiteDataUrl" value="https://hostname/umbraco/api/location/list?type=RecyclingSite"/>
			<add key="WasteTypesDataUrl" value="https://hostname/umbraco/api/wastetypes/list" />
		</appSettings>


Both of these are implemented as interfaces which can be replaced with alternative implementations.

One more entry in `appSettings` is required in `web.config`, which is a base URL used to redirect to the results page internally.

		<appSettings>
    		<add key="RecyclingSiteFinderBaseUrl" value="https://hostname/default.aspx" />
		<appSettings>

Results by distance are returned by a URL in the following format: 

	https://hostname/default.aspx?postcode=AB12CD&type=Anything

You can also request a list of sites without a postcode. This is intended for search engines to index the site pages:

	https://hostname/default.aspx?type=Anything

## NuGet package

This project also includes a NuGet package of the search form, which is designed to integrate with the legacy 'Topic' template in the `Escc.EastSussexGovUK.Umbraco` project. It uses the `RecyclingSiteFinderBaseUrl` setting to redirect to results from this application hosted outside of Umbraco.

## Development setup steps

1. From an Administrator command prompt, run `app-setup-dev.cmd` to set up a site in IIS.
2. Build the solution
3. In `~\web.config` uncomment and complete the `Proxy` section, and update the details for the locate API and the recycling sites API.