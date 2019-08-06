using Microsoft.WindowsAzure.Storage.Table;

namespace Migrate.AvgHigh.TableStorage.Progress
{
    public class ProgressEntity: TableEntity
    {
        public int State { get; set; }
    }
}
