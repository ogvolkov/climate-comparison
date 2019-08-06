using Microsoft.WindowsAzure.Storage.Table;

namespace Migrate.AvgHigh.TableStorage.AverageHigh
{
    public class AverageHighEntity: TableEntity
    {
        public double AverageHigh { get; set; }
    }
}
