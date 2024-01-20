using Neo4Fredis.Models.Entities;

namespace Neo4Fredis.Services.Implementation
{
    public interface IAuthService
    {
        public bool Add(string zainteresovan, string mesto, MestoZaIzlaske novoMesto);
    }
}
