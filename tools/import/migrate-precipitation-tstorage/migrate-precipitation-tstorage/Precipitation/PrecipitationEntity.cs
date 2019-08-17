using Microsoft.WindowsAzure.Storage.Table;

namespace Migrate.Precipitation.TableStorage.Precipitation
{
    public class PrecipitationEntity: TableEntity
    {
        public double Average { get; set; }
    }
}
