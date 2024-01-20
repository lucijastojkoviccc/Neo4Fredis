using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Neo4jClient;
using Neo4Fredis.Models.Entities;
using Neo4Fredis.Services.Implementation;
using System.Security.Claims;
using Neo4Fredis.Services.Implementation;

namespace Neo4Fredis.Services.Usage
{
    public class AuthService : IAuthService
    {
        private readonly IGraphClient client;

        public AuthService(IGraphClient client)
        {
            this.client = client;
        }

        private void DodajNaSpisak(Korisnik n, MestoZaIzlaske m)
        {
          //  n.SpisakMesta.Add(m); //da li da korisnik ima u sebi spisak mesta za koje je zainteresovan?
                                        //ako ne to onda moramo da osmislimo kako da dodaje i gde da se dodaje, na neki public spisak na nesto?
        }

        public bool Add(string zainteresovan, string mesto, MestoZaIzlaske novoMesto)
        {
            var M = novoMesto;
            this.client.Cypher.Match("user:Korisnik").Where((Korisnik user) => user.Email == zainteresovan)
                             .Create("(user)-[r:ZainteresovanZa]->(d:Mesto {M})")
                             .WithParam("M", M)
                             .ExecuteWithoutResultsAsync(); ;
            return true;
        }
    }
}
