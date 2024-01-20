
using Microsoft.AspNetCore.Mvc;
using Neo4Fredis.Hubs;
using Neo4Fredis.Models.DTOs;
using Neo4Fredis.Models.Entities;
using Neo4Fredis.Services.Implementation;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

namespace Neo4Fredis.Services.Usage
{
    public class ChatService : IChatServices
    {
        private readonly IConnectionMultiplexer _cmux;
        private readonly IHttpContextAccessor httpContextAccessor;

        //private readonly IDatabase _redisDb;
        private readonly string Channel = "Kanal";
        public ChatService(IConnectionMultiplexer cmux, IHttpContextAccessor httpContextAccessor)
        {
            _cmux = cmux;
            this.httpContextAccessor = httpContextAccessor;
            //_redisDb = redis.GetDatabase();
        }

        public string GetMessage()
        {
            throw new NotImplementedException();
        }
       
        public async Task SendMessage(string message)
        {
            var user = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var subscriber = _cmux.GetSubscriber();
            string A = string.Concat(user, "^");
            string B = string.Concat(A, message);
            IDatabase redis = _cmux.GetDatabase();
            CancellationToken ct = new CancellationToken();
            await subscriber.PublishAsync(Channel, B);
            redis.StringSet("Key", B);
            Thread.Sleep(2000);

        }

        private Task<string> GetMessageAsync()
        {
            var subscriber = _cmux.GetSubscriber();
            var tcs = new TaskCompletionSource<string>();
            subscriber.Subscribe(Channel, (channel, message) => tcs.TrySetResult(message));
            return tcs.Task;
        }

        private void RenderComplete(string json)
        {
            _tcs.TrySetResult(json);
        }

        private TaskCompletionSource<string> _tcs = new TaskCompletionSource<string>();
        
        public  async  Task<string> Receive() {

            IDatabase redis = _cmux.GetDatabase();
            var subscriber = _cmux.GetSubscriber();
            string[] slice;
            string A = await redis.StringGetAsync("Key");
            
            
           // slice = _tcs.Task.ToString();

            return A;

            
        }


        //dodato
        public void SaveMessage(string user, string message)
        {
            var db = _cmux.GetDatabase();
            var messageDto = new PorukaDTO { korisnik = user, poruka = message };
            db.ListLeftPush("messages", JsonSerializer.Serialize(messageDto));
        }

        public IEnumerable<PorukaDTO> GetMessages()
        {
            var db = _cmux.GetDatabase();
            var messages = db.ListRange("messages");

            return messages.Select(m => JsonSerializer.Deserialize<PorukaDTO>(m.ToString()));
        }
    }
}
