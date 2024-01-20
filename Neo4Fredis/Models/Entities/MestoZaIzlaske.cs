using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Neo4Fredis.Models.Entities
{
    public class MestoZaIzlaske
    {
        [Key]
        public int ID { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Adresa { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public int BrojZainteresovanih { get; set; } = 0;
        public List<Korisnik> ZainteresovaniKorisnici { get; set; } = new List<Korisnik>();
    }
}
