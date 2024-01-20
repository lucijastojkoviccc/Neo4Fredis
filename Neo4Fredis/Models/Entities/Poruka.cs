namespace Neo4Fredis.Models.Entities
{
    public class Poruka
    {
        public string? ID { get; set; }
        public string PorukaText { get; set; } = string.Empty;
        public string KorisnickoIme { get; set; } = string.Empty;
        public DateTime VremeSlanja { get; set; }
        public string CetId { get; set; } = string.Empty;

    }
}
