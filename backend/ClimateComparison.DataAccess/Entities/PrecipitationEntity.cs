using Microsoft.WindowsAzure.Storage.Table;

namespace ClimateComparison.DataAccess.Entities
{
    public class PrecipitationEntity : TableEntity
    {
        public double Average { get; set; }
    }
}
