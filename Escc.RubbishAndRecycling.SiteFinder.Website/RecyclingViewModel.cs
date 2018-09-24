using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Escc.EastSussexGovUK.Mvc;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// View model for the recycling sites finder
    /// </summary>
    /// <seealso cref="Escc.EastSussexGovUK.Mvc.BaseViewModel" />
    public class RecyclingViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the recycling sites matching a search
        /// </summary>
        public DataView RecyclingSites { get; set; }
    }
}