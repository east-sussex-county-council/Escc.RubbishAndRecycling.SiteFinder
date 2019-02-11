using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    public class FakeWasteTypesDataSource : IWasteTypesDataSource
    {
        public Task<IList<string>> LoadWasteTypes()
        {
            return Task.FromResult(new List<string>() {"Aluminium foil", "Aerosols", "Books", "Furniture", "Hosuehold waste"} as IList<string>);
        }
    }
}