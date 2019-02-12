using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Escc.EastSussexGovUK.Mvc;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// View model for the recycling sites finder
    /// </summary>
    /// <seealso cref="Escc.EastSussexGovUK.Mvc.BaseViewModel" />
    [Bind(Include ="Type,Postcode")]
    public class RecyclingViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the recycling sites matching a search
        /// </summary>
        public DataView RecyclingSites { get; set; }

        /// <summary>
        /// Gets or sets the types of waste that can be recycled
        /// </summary>
        public IList<string> WasteTypes { get; set; }

        /// <summary>
        /// Gets or sets the title that appears above the search form
        /// </summary>
        public string SearchFormTitle { get; set; }

        /// <summary>
        /// Gets or sets the selected waste type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the postcode submitted
        /// </summary>
        public string Postcode { get; set; }
    }
}