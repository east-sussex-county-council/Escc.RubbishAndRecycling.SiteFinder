using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    public class FakeWasteTypesDataSource : IWasteTypesDataSource
    {
        public IList<string> LoadWasteTypes()
        {
            return new List<string>() {"Aluminium foil", "Aerosols", "Books", "Furniture", "Hosuehold waste"};
        }
    }
}