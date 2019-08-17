using Microsoft.WindowsAzure.Storage.Table;

namespace Migrate.Precipitation.TableStorage.Progress
{
    public class ProgressEntity: TableEntity
    {
        public int State { get; set; }
    }
}
