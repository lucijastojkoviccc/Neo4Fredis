using Neo4jClient;
using Neo4jClient.Cypher;
using Neo4Fredis.Models.Entities;
using Neo4Fredis.Services.Implementation;
using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Neo4Fredis.Models.Entities;
using Neo4Fredis.Services.Implementation;
using Neo4Fredis.Services.Usage;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Diagnostics;
using Neo4Fredis.Models.DTOs;
using System.Net.Http;

namespace Neo4Fredis.Services.Usage
{

    public class MestoService : IMestoService
    {
        private readonly IGraphClient client;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MestoService(IGraphClient client, IHttpContextAccessor httpContextAccessor)
        {
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;
        }

        public bool AddPlace(MestoZaIzlaske novoMesto)
        {
            var novo = new MestoZaIzlaske
            {
                Naziv = novoMesto.Naziv,
                Adresa = novoMesto.Adresa,
                Opis = novoMesto.Opis,
                BrojZainteresovanih=1
            };
            this.client.Cypher.Create("(m:MestoZaIzlaske $novoMesto)")
                           .WithParam("novoMesto", novoMesto)
                           .ExecuteWithoutResultsAsync(); ;

            var b = this.client.Cypher.Match("(m: MestoZaIzlaske)")
                .Where((MestoZaIzlaske m) =>
                m.Naziv == novoMesto.Naziv)
                .Return(m => m.As<MestoZaIzlaske>())
                .ResultsAsync;

            var a = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (b.Result != null)
            {
                this.client.Cypher
                            .Match("(m:MestoZaIzlaske), (user:Korisnik)")

                            .Where((MestoZaIzlaske m, Korisnik user) =>
                                 m.Naziv == novoMesto.Naziv &&
                                 user.Email == a)
                            .Create("(user)-[r:ZainteresovanZa]->(m)")

                            .ExecuteWithoutResultsAsync();
            }
            return true;

        }

        public bool Delete(string Naziv)
        {
            //throw new NotImplementedException();
            this.client.Cypher.OptionalMatch("(m:MestoZaIzlaske)<-[r]-()")
                              .Where((MestoZaIzlaske m) => m.Naziv == Naziv)
                              .Delete("r, m")
                              .ExecuteWithoutResultsAsync();
            return true;
        }

        public MestoZaIzlaske FindByName()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MestoZaIzlaske> GetAll()
        {
            //throw new NotImplementedException();
            var mesta = this.client.Cypher
                            .Match("(x: MestoZaIzlaske)")
                            .Return(x => x.As<MestoZaIzlaske>())
                            .ResultsAsync;

            return mesta.Result;
        }
        public IEnumerable<MestoZaIzlaske> GetMyPlaces(HttpContext httpContext)
        {
            var loggedInUserEmail = httpContext.User.FindFirstValue(ClaimTypes.Email);
            var b = this.client.Cypher
                   .OptionalMatch("(korisnik:Korisnik)-[r:ZainteresovanZa]->(mesto:MestoZaIzlaske)")
                   .Where((Korisnik korisnik, MestoZaIzlaske mesto) =>
                   korisnik.Email == loggedInUserEmail)
                   .Return(mesto => mesto.As<MestoZaIzlaske>())
                   .ResultsAsync;


            return b.Result;
        }
        public IEnumerable<MestoZaIzlaske> GetUsersPlacesPage(string korisnickoIme)
        {
           
            var b = this.client.Cypher
                   .Match("(korisnik:Korisnik)-[r:ZainteresovanZa]->(mesto:MestoZaIzlaske)")
                    .Where((Korisnik korisnik) => korisnik.KorisnickoIme == korisnickoIme)
                    .Return(mesto => mesto.As<MestoZaIzlaske>())
                    .ResultsAsync;


            return b.Result;
        }

        public bool PrijaviSe(string mesto, string KorisnikEmail)
        {
            //throw new NotImplementedException();
            var mestoo = this.client.Cypher
                            .Match("(m:MestoZaIzlaske)")
                            .Where((MestoZaIzlaske m) => m.Naziv == mesto)
                            .Return(m => m.As<MestoZaIzlaske>()).ResultsAsync;


            if (mestoo == null)
                return false;

            var tajkorisnik = this.client.Cypher
                                     .Match("(kor:Korisnik)")
                                     .Where((Korisnik kor) => kor.Email == KorisnikEmail)
                                     .Return(kor => kor.As<Korisnik>());

            if (tajkorisnik == null)
                return false;

            MestoZaIzlaske mojeM = new MestoZaIzlaske();
            //mojD.Naziv = dog;
            //if (tajngo == null)
            //    return false;

            //var user = this.client.Cypher
            //                .Match("(korisnik:Korisnik)")
            //                .Match("(m:MestoZaIzlaske)")
            //                .Where((Korisnik korisnik) => korisnik.Email == KorisnikEmail)
            //                .Where((MestoZaIzlaske m) => m.Naziv == mesto)
            //                .Create("(korisnik)-[:ZainteresovanZa]->(m)");

            //Debug.WriteLine("USO SAM");
            // .Return(korisnik => korisnik.As<Korisnik>()).ResultsAsync;

            Korisnik mojK = new Korisnik();
            mojK.Email = KorisnikEmail;
            
           
            mojeM.ZainteresovaniKorisnici.Add(mojK);
               
            mojeM.BrojZainteresovanih++;
            

            //this.client.Cypher
            //    .Match("(korisnik:Korisnik)", "(mesto:MestoZaIzlaske)")
            //    .Where((Korisnik korisnik) => korisnik.Email == korisnik.Email)
            //    .AndWhere((MestoZaIzlaske mesto) => mesto.Naziv == meston)
            //    .Create("(korisnik)-[:ZainteresovanZa]->(mesto)")
            //    .ExecuteWithoutResultsAsync();
            //this.client.Cypher
            //     .Match("(mesto:MestoZaIzlaske)")
            //     .Where((MestoZaIzlaske m) => m.Naziv == mesto)
            //     .Set("mesto.BrojZainteresovanih = mesto.BrojZainteresovanih + 1")
            //     .ExecuteWithoutResultsAsync();

            return true;
        }

        public async Task<IEnumerable<Sortiranje>> UredjenaMesta(IConnectionMultiplexer _redis)
        {
            var DB = _redis.GetDatabase();

            var sortedMesta = await DB.SortedSetRangeByRankWithScoresAsync("najpopularnije: mesto", 0, -1, Order.Descending);


            var rezultati = new List<Sortiranje>();

            foreach (var sortedItem in sortedMesta)
            {
                rezultati.Add(new Sortiranje() { imeMesta = sortedItem.Element, broj = sortedItem.Score });
            }

            return rezultati;
        }

    }
}
