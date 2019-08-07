using Microsoft.WindowsAzure.Storage.Table;

namespace ClimateComparison.DataAccess.Entities
{
    public class AverageHighEntity : TableEntity
    {
        public double AverageHigh { get; set; }
    }
}
