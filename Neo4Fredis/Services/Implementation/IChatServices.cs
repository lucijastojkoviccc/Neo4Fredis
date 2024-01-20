using Neo4Fredis.Models.DTOs;
using Neo4Fredis.Models.Entities;

namespace Neo4Fredis.Services.Implementation
{
    public interface IChatServices
    {
        public Task SendMessage(string message);
        public Task<string> Receive();

        //dodato
        public void SaveMessage(string user, string message);
        public IEnumerable<PorukaDTO> GetMessages();
    }
}
