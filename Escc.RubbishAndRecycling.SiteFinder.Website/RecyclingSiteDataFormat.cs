using System.Data;
using System.Globalization;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// Recycling site data is managed in a DataSet format for the search. This creates the DataSet.
    /// </summary>
    public static class RecyclingSiteDataFormat
    {
        public static DataSet CreateDataSet()
        {
             // Dataset to hold the library table
            using (DataSet ds = new DataSet())
            {
                ds.Locale = CultureInfo.CurrentCulture;

                using (DataTable dt = new DataTable())
                {
                    dt.Locale = CultureInfo.CurrentCulture;
                    DataColumn dcName = new DataColumn("Title");
                    dt.Columns.Add(dcName);
                    DataColumn url = new DataColumn("URL");
                    dt.Columns.Add(url);
                    DataColumn lat = new DataColumn("Latitude");
                    dt.Columns.Add(lat);
                    DataColumn lon = new DataColumn("Longitude");
                    dt.Columns.Add(lon);
                    //add the table to our dataset and accept all changes
                    ds.Tables.Add(dt);

                    ds.AcceptChanges();
                }
                
                return ds;
            }
        }
    }
}