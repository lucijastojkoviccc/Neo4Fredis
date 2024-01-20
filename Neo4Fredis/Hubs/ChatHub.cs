using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Neo4Fredis.Models.DTOs;
using Neo4Fredis.Models.Entities;
using Neo4Fredis.Services.Implementation;
using Neo4Fredis.Services.Usage;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

namespace Neo4Fredis.Hubs
{
    public class ChatHub : Hub
    {
        
        private readonly IKorisnikService _korService;
        private readonly IChatServices _chatService;

        
        private readonly IConnectionMultiplexer _cmux;
     

        private readonly  string Chanell = "NBP";
        public ChatHub(IConnectionMultiplexer cmux, IKorisnikService k, IChatServices chatService)
        {
          
            _chatService = chatService;
            _korService = k;
            _cmux = cmux;
         
        }

        public async Task Pub(string user, string messageString) {

            var subscriber =  _cmux.GetSubscriber();
            string A = string.Concat(user, "^");
            string B  = string.Concat(A, messageString);
            
            await subscriber.PublishAsync(Chanell, B);
         

        }

        public async Task<PorukaDTO> Sub() {
            var subscriber = _cmux.GetSubscriber();
            string korisnik = string.Empty;
            string poruka = string.Empty;
            List<string> listaPoruka = new List<string>();
            await subscriber.SubscribeAsync(Chanell, (channel, porukaPLUScovek) => {
                //listaPoruka.Add(message);
                string[] subs = porukaPLUScovek.ToString().Split("^");
                korisnik = subs[0];
                poruka = subs[1];
                Console.WriteLine("Poruku sa tekstom: " + poruka + " salje korisnik: " + korisnik);
            });
            Console.ReadLine();

            PorukaDTO vratiPoruku = new PorukaDTO();
            vratiPoruku.korisnik = korisnik;
            vratiPoruku.poruka = poruka;

            return vratiPoruku;
        }

        public async Task SendMessageNovo(string user, string messageString)
        {
            //var message = JsonSerializer.Deserialize<Message>(messageString);

            await Clients.All.SendAsync("NBP_Chat", user, messageString);
        }


        public string korisnik() => Context.User.FindFirstValue(ClaimTypes.Email);

        public string re() => Context.User.FindFirstValue(ClaimTypes.Email);

        public string ren() => Context.User.FindFirstValue(ClaimTypes.Email);
        public string rek() => Context.User.FindFirstValue(ClaimTypes.Email);
        
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);

            // Čuvanje poruke u Redis-u
            _chatService.SaveMessage(user, message);
        }
        public async Task<IEnumerable<PorukaDTO>> GetMessages()
        {
            // Dohvatanje poruka iz Redis-a
            return _chatService.GetMessages();
        }
    }
}
