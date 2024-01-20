using Neo4jClient;
using Neo4Fredis.Models.Entities;
using Neo4Fredis.Services.Implementation;

namespace Neo4Fredis.Services.Usage
{
    public class KorisnikService : IKorisnikService
    {
        private readonly IGraphClient client;

        public KorisnikService(IGraphClient client) { 
            this.client = client;
        }

        public bool AddKorisnik(Korisnik noviKorisnik)
        {
            this.client.Cypher.Create("(k:Korisnik $noviKorisnik)")
                                 .WithParam("noviKorisnik", noviKorisnik)
                                 .ExecuteWithoutResultsAsync(); ;

            return true;
        }

        public IEnumerable<Korisnik> SviZainteresovani(string imeMesta)
        {
            var b = this.client.Cypher
                    .OptionalMatch("(korisnik:Korisnik)-[r:ZainteresovanZa]->(mesto:MestoZaIzlaske)")
                    .Where((Korisnik korisnik, MestoZaIzlaske mesto) =>
                    mesto.Naziv == imeMesta)
                    .Return(korisnik => korisnik.As<Korisnik>())
                    .ResultsAsync;


            return b.Result;
        }
        public async Task<Korisnik> KorisniciPrikazi(string k)
        {

            var query = this.client.Cypher
                 .Match("(korisnik:Korisnik {korisnickoIme: {korisnickoImeParam}})")
                 .WithParam("korisnickoImeParam", k)
                 .Return(korisnik => korisnik.As<Korisnik>())
                 .ResultsAsync;

            return query.Result.FirstOrDefault();

        }
        public IEnumerable<Korisnik> SviKorisnici(HttpContext httpContext)
        {
          

            var query = this.client.Cypher
               .Match("(korisnik:Korisnik)")
               .Return(korisnik => korisnik.As<Korisnik>())
               .ResultsAsync;

            return query.Result;
        }

       
    }
}
