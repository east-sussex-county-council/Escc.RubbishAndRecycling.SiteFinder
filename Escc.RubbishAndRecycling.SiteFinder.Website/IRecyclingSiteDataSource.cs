using System.Data;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// A data source providing recycling site details for the find feature
    /// </summary>
    public interface IRecyclingSiteDataSource
    {
        /// <summary>
        /// Adds the recycling sites from the data source.
        /// </summary>
        /// <param name="table">The table.</param>
        void AddRecyclingSites(DataTable table);
    }
}