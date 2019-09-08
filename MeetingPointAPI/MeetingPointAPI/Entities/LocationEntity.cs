namespace MeetingPointAPI.Entities
{
    public class LocationEntity
    {
        public int Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Vicinity { get; set; }
        public string Icon { get; set; }
        public string Href { get; set; }
        public int? Distance { get; set; }
        public string Category { get; set; }
    }
}
