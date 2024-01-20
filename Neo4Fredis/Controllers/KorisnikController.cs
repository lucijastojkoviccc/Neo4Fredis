using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Neo4jClient.Cypher;
using System.Xml.Linq;
using System.Collections;
using Neo4Fredis.Models.Entities;
using Neo4j.Driver;
using System.Collections.Generic;
using StackExchange.Redis;
using Neo4Fredis.Services.Implementation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Neo4Fredis.Models.DTOs;
using static ServiceStack.Diagnostics.Events;
using System.Linq;

namespace Neo4Fredis.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    public class KorisnikController : Controller
    {

        private readonly IKorisnikService korisnikService;
        private readonly IGraphClient _client;
        private readonly IConnectionMultiplexer _redis;
        private readonly IHttpContextAccessor httpContextAccessor;
        public KorisnikController(IHttpContextAccessor httpContextAccessor, IConnectionMultiplexer _redis, IKorisnikService korisnikService, IGraphClient _client)
        {
            this.korisnikService = korisnikService;
            this._redis = _redis;
            this._client = _client;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Korisnik()
        {
            return View();
        }

        [HttpPost]
        //[Route("Login")]
        public async Task<IActionResult> Login(string username, string lozinka)
        {

            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Email, username),
                    new Claim(ClaimTypes.Role, "Korisnik")
                };

            ClaimsIdentity identitycl = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            ClaimsPrincipal principalcl = new ClaimsPrincipal(identitycl);

            var properties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principalcl,
                properties
            );

            return RedirectToAction("LoggedInKorisnik", "Korisnik");
        }

        [HttpPost]
        //[Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("AddKorisnikPage");
        }

        [HttpGet]
        public IActionResult AddKorisnikPage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoggedInKorisnik()
        {
            return View();
        }

        public IActionResult SviZainteresovani()
        {
            return View();
        }

        public IActionResult KorisniciPrikazi()
        {

           // var podaci = this.korisnikService.KorisniciPrikazi();
            return View();
           
        }


        //[HttpGet]
        //public async Task<IActionResult> KorisniciPrikazi(string k)
        //{
        //    var query = await _client.Cypher
        //            .Match("(korisnik:Korisnik)")
        //            .Where((Korisnik korisnik) => korisnik.KorisnickoIme == k)                
        //            .Return(korisnik => korisnik.As<Korisnik>())
        //            .s
          

           

        //    return View(query);

            
        //}
        [HttpGet]
        public async Task<IActionResult> KorisniciPrikazi(string k)
        {

            var query = await _client.Cypher
            .Match("(korisnik:Korisnik)")
            .Where($"korisnik.KorisnickoIme IS NOT NULL AND korisnik.KorisnickoIme = '{k}'")
            .Return(korisnik => korisnik.As<Korisnik>())
            .ResultsAsync;

            var korisnik = query.SingleOrDefault(); // Use SingleOrDefault to get a single result or null

            if (korisnik == null)
            {
                // Handle the case where the user with the specified korisnickoIme is not found
                return RedirectToAction("UserNotFound");
            }

            return View(korisnik);

        }

        public IActionResult SviKorisnici()
        {
            var podaci = this.korisnikService.SviKorisnici(HttpContext);
            return View(podaci);
        }

        [HttpGet]
        public IActionResult LoginKorisnik()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Postoji()
        {
            return View();
        }

        [HttpPost]
        //[Route("Add")]
        public IActionResult Add(Korisnik model)
        {

            var ngo = this.korisnikService.AddKorisnik(model);
            if (ngo)
            {
                return RedirectToAction("AddKorisnikPage", "Korisnik");
            }
            return View();
        }

        [HttpGet]
        
        public async Task<IActionResult> SviZainteresovani(MestoZaIzlaske m)
        {
            var b = await this._client.Cypher
                    .OptionalMatch("(korisnik:Korisnik)-[r:ZainteresovanZa]->(mesto:MestoZaIzlaske)")
                    .Where((Korisnik korisnik, MestoZaIzlaske mesto) =>
                    mesto.Naziv == m.Naziv)
                    .Return(korisnik => korisnik.As<Korisnik>())
                    .ResultsAsync;
            

            return View(b);       
        }

        //[HttpGet]
        //public async Task<IActionResult> SviKorisnici()
        //{
        //    var b = await this._client.Cypher
        //            .OptionalMatch("(korisnik:Korisnik)-[r:ZainteresovanZa]->(mesto:MestoZaIzlaske)")
                    
        //            .Return(korisnik => korisnik.As<Korisnik>())
        //            .ResultsAsync;

        //    return View(b);
        //}
        //[HttpGet]
        //public IActionResult SviKorisnici()
        //{
        //    var query = this._client.Cypher
        //        .Match("(korisnik:Korisnik)")
        //        .Return(korisnik => korisnik.As<Korisnik>())
        //        .ResultsAsync;


        //    return View(query);
        //}


        [HttpPost]
        
        public async Task<IActionResult> UpdateKor(PromeniLozinku k)
        {
            var a = this.httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);

            var result = await _client.Cypher.Match("(kk:Korisnik)")
                                            .Where((Korisnik kk) => kk.Email == a)
                                            .Return(kk => kk.As<Korisnik>()).ResultsAsync;

            Korisnik stariKor = result.First();
            stariKor.Lozinka = k.NovaLozinka;

            await this._client.Cypher
            .Match("(k1:Korisnik)")
            .Where((Korisnik k1) => k1.Email == a)
            .Set("k1 = $korisnik")
            .WithParam("korisnik", stariKor)
            .ExecuteWithoutResultsAsync();

            // return NoContent();
            return RedirectToAction("LoggedInKorisnik");
        }

        [HttpGet]
        public IActionResult UpdateKorisnik()
        {

            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
