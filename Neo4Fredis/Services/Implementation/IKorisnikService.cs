using Neo4Fredis.Models.Entities;

namespace Neo4Fredis.Services.Implementation
{
    public interface IKorisnikService
    {
        public bool AddKorisnik(Korisnik noviKorisnik);
        public IEnumerable<Korisnik> SviZainteresovani(string imeDogadjaja);

        public IEnumerable<Korisnik> SviKorisnici(HttpContext httpContext);

        public Task<Korisnik> KorisniciPrikazi(string k);

    }
}
