using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Neo4Fredis.Models.DTOs;
using Neo4Fredis.Models.Entities;
using Neo4Fredis.Services.Implementation;
using StackExchange.Redis;
using System.Security.Claims;
using System.Diagnostics;
using Neo4Fredis.Services.Usage;
using ServiceStack;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;

namespace Neo4Fredis.Controllers
{
    //[Microsoft.AspNetCore.Mvc.Route("MestoZaIzlaske")]
    public class MestoZaIzlaskeController : Controller
    {
       
        private readonly ILogger<MestoZaIzlaskeController> _logger;
        private readonly IGraphClient _client;
        private readonly IMestoService mesto_service;
        private readonly IKorisnikService korisnik_service;
        private readonly IConnectionMultiplexer _redis;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MestoZaIzlaskeController(IHttpContextAccessor httpContextAccessor, IConnectionMultiplexer _redis, IKorisnikService korisnik_service, IMestoService mesto_service, IGraphClient client, ILogger<MestoZaIzlaskeController> logger)
        {
            _client = client;
            _logger = logger;
            this.mesto_service = mesto_service;
            this.korisnik_service = korisnik_service;
            this._redis = _redis;
            this.httpContextAccessor = httpContextAccessor;
        }
        public IActionResult MestoZaIzlaske()
        {
            return View();
        }

        public IActionResult Brisanje()
        {
            return View();
        }


        //[HttpPost]
        //public IActionResult AddPlace(MestoZaIzlaske model)
        //{

        //    var addEventResultSuccess = this.mesto_service.AddPlace(model);

        //    if (addEventResultSuccess)
        //    {
        //        TempData["msg"] = "Added Successfully";
        //        return RedirectToAction("GetAll", "MestoZaIzlaske");

        //    }

        //    TempData["msg"] = "Error has occured on server side";

        //    return View(TempData);
        //}
        [HttpPost]
        public IActionResult AddPlace(MestoZaIzlaske model)
        {

            var addEventResultSuccess = this.mesto_service.AddPlace(model);

            if (addEventResultSuccess)
            {
                TempData["msg"] = "Added Successfully";
                var DB = _redis.GetDatabase();
                Task.WhenAll(
                    DB.SortedSetAddAsync("najpopularnije: mesto", model.Naziv, 1)
                );
                int b1 = 1;
                this._client.Cypher
               .Match("(mesto:MestoZaIzlaske)")
               .Where((MestoZaIzlaske mesto) => mesto.Naziv == model.Naziv)
               .Set("mesto.BrojZainteresovanih = $b1")
               .WithParam("b1", b1)
               .ExecuteWithoutResultsAsync();

                return RedirectToAction("GetAll", "MestoZaIzlaske");
            }

            TempData["msg"] = "Error has occured on server side";


            return View(TempData);
        }

        //[HttpPost]

        //public async Task<IActionResult> PrijaviSe(ZainteresovanZa p)
        //{
        //    var a = this.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);



        //    var b = await this._client.Cypher
        //        .OptionalMatch("(korisnik:Korisnik)-[r:ZainteresovanZa]->(mesto:MestoZaIzlaske)")
        //        .Where((Korisnik korisnik, MestoZaIzlaske mesto) =>
        //        korisnik.Email == a &&
        //        mesto.Naziv == p.ImeMesta)
        //        .Return(korisnik => korisnik.As<Korisnik>())
        //        .ResultsAsync;


        //    //bool uspeh = mesto_service.PrijaviSe(b., model.LokacijaMesta, model.KorisnikEmail);


        //    if (b.First() == null)
        //    {

        //        await this._client.Cypher
        //                        .Match("(m:MestoZaIzlaske), (k:Korisnik)")
        //                        .Where((MestoZaIzlaske m, Korisnik k) =>
        //                             m.Naziv == p.ImeMesta &&
        //                             k.Email == a
        //                             )

        //                        .Create("(k)-[r:ZainteresovanZa]->(m)")

        //                        .ExecuteWithoutResultsAsync();

        //        return RedirectToAction("UspesnaPrijava", "MestoZaIzlaske");
        //    }


        //    return RedirectToAction("NeuspesnaPrijava", "MestoZaIzlaske");

        //}

        //public async Task<IActionResult> Delete(MestoZaIzlaske d1)
        //{
        //    await this._client.Cypher.OptionalMatch("(d: MestoZaIzlaske)<-[r]-()")
        //                      .Where((MestoZaIzlaske d) => d.Naziv == d1.Naziv)
        //                      .Delete("r, d")
        //                      .ExecuteWithoutResultsAsync();

        //    return RedirectToAction("GetAll");
        //}
        //[HttpPost]
        //public async Task<IActionResult> PrijaviSe(ZainteresovanZa p)
        //{
        //    var a = this.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        //    var b = await this._client.Cypher
        //        .OptionalMatch("(korisnik:Korisnik)-[r:ZainteresovanZa]->(mesto:MestoZaIzlaske)")
        //        .Where((Korisnik korisnik, MestoZaIzlaske mesto) =>
        //        korisnik.Email == a &&
        //        mesto.Naziv == p.ImeMesta)
        //        .Return(korisnik => korisnik.As<Korisnik>())
        //        .ResultsAsync;

        //    if (b.First() == null)
        //    {
        //        await this._client.Cypher
        //                        .Match("(m:MestoZaIzlaske), (k:Korisnik)")
        //                        .Where((MestoZaIzlaske m, Korisnik k) =>
        //                             m.Naziv == p.ImeMesta &&
        //                             k.Email == a
        //                             )

        //                        .Create("(k)-[r:ZainteresovanZa]->(m)")
        //                        .ExecuteWithoutResultsAsync();
        //        var DB = _redis.GetDatabase();

        //        var k = await this._client.Cypher
        //            .Match("(mesto:MestoZaIzlaske)")
        //            .Where((MestoZaIzlaske mesto) => mesto.Naziv == p.ImeMesta)
        //            .Return(mesto => mesto.As<MestoZaIzlaske>())
        //            .ResultsAsync;

        //        await Task.WhenAll(
        //            DB.SortedSetAddAsync("najpopularnije: mesto", k.First().Naziv, k.First().BrojZainteresovanih)
        //        );

        //        return RedirectToAction("UspesnaPrijava", "MestoZaIzlaske");
        //    }


        //    return RedirectToAction("NeuspesnaPrijava", "MestoZaIzlaske");
        //}

        public async Task<IActionResult> PrijaviSe(ZainteresovanZa p)
        {
            var a = this.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var b = await this._client.Cypher
                .OptionalMatch("(korisnik:Korisnik)-[r:ZainteresovanZa]->(mesto:MestoZaIzlaske)")
                .Where((Korisnik korisnik, MestoZaIzlaske mesto) =>
                korisnik.Email == a &&
                mesto.Naziv == p.ImeMesta)
                .Return(korisnik => korisnik.As<Korisnik>())
                .ResultsAsync;


            if (b.First() == null)
            {


                await this._client.Cypher
                                .Match("(m:MestoZaIzlaske), (k:Korisnik)")
                                .Where((MestoZaIzlaske m, Korisnik k) =>
                                     m.Naziv == p.ImeMesta &&
                                     k.Email == a
                                     )

                                .Create("(k)-[r:ZainteresovanZa]->(m)")
                                .ExecuteWithoutResultsAsync();
                IEnumerable<Korisnik> bx = await this._client.Cypher
                  .OptionalMatch("(korisnik:Korisnik)-[r:ZainteresovanZa]->(mesto:MestoZaIzlaske)")
                  .Where((Korisnik korisnik, MestoZaIzlaske mesto) =>
                  mesto.Naziv == p.ImeMesta)
                  .Return(korisnik => korisnik.As<Korisnik>())
                  .ResultsAsync;

                int b1 = bx.Count();

                await this._client.Cypher
                  .Match("(mesto:MestoZaIzlaske)")
                  .Where((MestoZaIzlaske mesto) => mesto.Naziv == p.ImeMesta)
                  .Set("mesto.BrojZainteresovanih = $b1")
                  .WithParam("b1", b1)
                  .ExecuteWithoutResultsAsync();


                var DB = _redis.GetDatabase();

                var k = await this._client.Cypher
                    .Match("(mesto:MestoZaIzlaske)")
                    .Where((MestoZaIzlaske mesto) => mesto.Naziv == p.ImeMesta)
                    .Return(mesto => mesto.As<MestoZaIzlaske>())
                    .ResultsAsync;

                await Task.WhenAll(
                    DB.SortedSetAddAsync("najpopularnije: mesto", k.First().Naziv, b1)
                );

                return RedirectToAction("UspesnaPrijava", "MestoZaIzlaske");
            }


            return RedirectToAction("NeuspesnaPrijava", "MestoZaIzlaske");
        }
        public IActionResult PrijaviSePage()
        {
            return View();
        }

        public IActionResult UspesnaPrijava()
        {
            return View();
        }

        public IActionResult NeuspesnaPrijava()
        {
            return View();
        }

        public IActionResult GetAll()
        {

            var podaci = this.mesto_service.GetAll();
            return View(podaci);
        }



        public IActionResult AddPlacePage()
        {

            return View();
        }

        public IActionResult Prijavi()
        {
            return View();
        }
        public IActionResult PrikazPrijavljenih()
        {
            return View();
        }

        public IActionResult GetMyPlaces()
        {

            var podaci = this.mesto_service.GetMyPlaces(HttpContext);
            return View(podaci);
        }
        //public IActionResult GetUsersPlacesPage()
        //{
        //    return View();
        //}
        public IActionResult GetUsersPlacesPage(string korisnickoIme)
        {

            var podaci = this.mesto_service.GetUsersPlacesPage(korisnickoIme);
            return View(podaci);
        }


        [HttpPost]
        public async Task<NoContentResult> UrediPoBrojuZainteresovanih()
        {

            var mesta = this._client.Cypher
                                .Match("(x: MestoZaIzlaske)")
                                .Return(x => x.As<MestoZaIzlaske>())
                                .ResultsAsync;
            var DB = _redis.GetDatabase();

            foreach (MestoZaIzlaske m in mesta.Result)
            {
                await Task.WhenAll(
                    DB.SortedSetAddAsync("K10", m.Naziv, m.BrojZainteresovanih)
                );
            }

            return NoContent();

        }

        public async Task<IActionResult> UbaciUSortiranje(Sortiranje u)
        {
            var DB = _redis.GetDatabase();

            var k = await this._client.Cypher
                .Match("(mesto:MestoZaIzlaske)")
                .Where((MestoZaIzlaske mesto) => mesto.Naziv == u.imeMesta)
                .Return(mesto => mesto.As<MestoZaIzlaske>())
                .ResultsAsync;

            await Task.WhenAll(
                DB.SortedSetAddAsync("najpopularnije: mesto", k.First().Naziv, k.First().BrojZainteresovanih)
            );

            return NoContent();
        }
        public async Task<List<Sortiranje>> UredjenaMesta()
        {
            var DB = _redis.GetDatabase();

            var sortedMesta = await DB.SortedSetRangeByRankWithScoresAsync("najpopularnije: mesto", 0, -1, Order.Descending); //"kljuc_zainteresovani"

            var rezultati = new List<Sortiranje>();

            foreach (var sortedItem in sortedMesta)
            {
                var mesto = await this._client.Cypher
                    .Match("(s:Sortiranje)")
                    .Where((Sortiranje s) => s.imeMesta == sortedItem.Element)
                    .Return(mesto => mesto.As<Sortiranje>())
                    .ResultsAsync;

                rezultati.Add(mesto.First());
            }

            return rezultati;
        }

        public async Task<IActionResult> UredjenaMestaPage()
        {
            var podaci = await this.mesto_service.UredjenaMesta(_redis);
            return View(podaci);
        }
    }
}
