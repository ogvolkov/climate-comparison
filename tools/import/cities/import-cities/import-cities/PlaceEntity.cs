using Microsoft.WindowsAzure.Storage.Table;

namespace ClimateComparison.Import.Cities
{
    public class PlaceEntity: TableEntity
    {
        public string Id { get; set; }

        public string AltName { get; set; }

        public string Name { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public string CountryCode { get; set; }

        public int Population { get; set; }
    }
}
