using System;
using System.Data;
using System.Threading.Tasks;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    public class FakeRecyclingSiteDataSource : IRecyclingSiteDataSource
    {
        public Task AddRecyclingSites(DataTable table)
        {
            if (table == null) throw new ArgumentNullException("table");

            var lewes = table.NewRow();
            lewes["Title"] = "Lewes Household Waste Recycling Site";
            lewes["URL"] = "/recyclingsites/lewes.htm";
            lewes["Latitude"] = "50.872872";
            lewes["Longitude"] = "0.013526";
            table.Rows.Add(lewes);

            var uckfield = table.NewRow();
            uckfield["Title"] = "Maresfield Household Waste Recycling Site";
            uckfield["URL"] = "/recyclingsites/maresfield.htm";
            uckfield["Latitude"] = "50.972144";
            uckfield["Longitude"] = "0.096452";
            table.Rows.Add(uckfield);

            var crowborough = table.NewRow();
            crowborough["Title"] = "Crowborough recycling point";
            crowborough["URL"] = "/recyclingsites/crowborough.htm";
            crowborough["Latitude"] = "51.058695";
            crowborough["Longitude"] = "0.158844";
            table.Rows.Add(crowborough);

            return null;
        }
    }
}