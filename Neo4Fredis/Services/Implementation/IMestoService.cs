using Neo4Fredis.Models.DTOs;
using Neo4Fredis.Models.Entities;
using StackExchange.Redis;

namespace Neo4Fredis.Services.Implementation
{
    public interface IMestoService
    {
        public IEnumerable<MestoZaIzlaske> GetAll();
        public bool AddPlace(MestoZaIzlaske novoMesto);

        public bool Delete(string Naziv);
        public MestoZaIzlaske FindByName();
        public bool PrijaviSe(string mesto, string KorisnikEmail);

        public IEnumerable<MestoZaIzlaske> GetMyPlaces(HttpContext httpContext);

        public IEnumerable<MestoZaIzlaske> GetUsersPlacesPage(string korisnickoIme);

        public Task<IEnumerable<Sortiranje>> UredjenaMesta(IConnectionMultiplexer _redis);
    }
}
