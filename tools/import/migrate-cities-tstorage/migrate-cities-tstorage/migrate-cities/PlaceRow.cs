namespace ClimateComparison.Migrate.Cities
{
    public class PlaceRow
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AltNames { get; set; }

        public float Longitude { get; set; }

        public float Latitude { get; set; }

        public int Population { get; set; }

        public string CountryCode { get; set; }
    }
}
