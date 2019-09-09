using Microsoft.WindowsAzure.Storage.Table;

namespace ClimateComparison.Migrate.Cities.Progress
{
    public class ProgressEntity: TableEntity
    {
        public int State { get; set; }
    }
}
