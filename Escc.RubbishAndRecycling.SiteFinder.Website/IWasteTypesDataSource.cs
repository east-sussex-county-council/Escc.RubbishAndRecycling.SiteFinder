using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// A way to get a list of all waste types which may be accepted
    /// </summary>
    public interface IWasteTypesDataSource
    {
        /// <summary>
        /// Gets a list of all waste types which may be accepted
        /// </summary>
        /// <returns></returns>
        Task<IList<string>> LoadWasteTypes();
    }
}
