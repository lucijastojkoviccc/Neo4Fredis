using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Neo4Fredis.Models.Entities
{
    public class Korisnik
    {
        [Key]
        public int ID { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;

        [Required]
        public string KorisnickoIme { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;
        public string Role { get; set; } = "Korisnik";
       // public string ProfilnaSlika { get; set; } = string.Empty;
       
    }
}
