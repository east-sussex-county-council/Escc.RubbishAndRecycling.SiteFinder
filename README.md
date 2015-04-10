Escc.RubbishAndRecycling.SiteFinder.Website
===========================================

Search facility for waste and recycling sites in East Sussex. 

This tool relies on two external tools: 

* a web service, referenced from the `Escc.FormControls` project, which can look up a postcode and return a latitude and longitude for the centre point.
* an API, defined in the `Escc.CustomerFocusTemplates.Website` project, which provides recycling site data as JSON.

Both of these are implemented as interfaces which can be replaced with alternative implementations.

## Development setup steps

1. Install [NuBuild](https://github.com/bspell1/NuBuild)
2. From an Administrator command prompt, run `app-setup-dev.cmd` to set up a site in IIS.
3. Build the solution
9. In `~\web.config` uncomment and complete the `Proxy` section, and update the three URLs for the web service, library data API and mobile library times page.