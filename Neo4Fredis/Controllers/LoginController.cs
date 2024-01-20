using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Neo4Fredis.Models.Entities;
using System.Security.Claims;

namespace Neo4Fredis.Controllers
{
    public class LoginController : Controller
    {
        private readonly IGraphClient client;

        public LoginController(IGraphClient client) {
            this.client = client;
        }
        public IActionResult LoginPage()
        {
            return View();
        }
        public IActionResult LoginKorisnik()
        {
            return View();
        }
    

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            //msm da je ok da se ima Login_kao jer eto kao imamo Korisnika i onda tu nesto smo kao cool idk
            if (login.Login_Kao == "Korisnik")
            {
                var k = await this.client.Cypher
              .Match("(korisnik:Korisnik)")
              .Where((Korisnik korisnik) =>
               korisnik.Email == login.Email &&
               korisnik.Lozinka == login.Lozinka)
              .Return(korisnik => korisnik.As<Korisnik>())
              .ResultsAsync;

                if (k.First() == null)
                {
                    return RedirectToAction("VecPostoji", "Korisnik");
                }
            }


        var claims = new List<Claim> {
                    new Claim(ClaimTypes.Email, login.Email)//,
                    //new Claim(ClaimTypes.Name, login_kao),
                
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

            if(User.Identity.IsAuthenticated)
                return RedirectToAction("LoggedInKorisnik", "Korisnik");
            else
                return RedirectToAction("LoginPage", "Login");
            return RedirectToAction("Korisnik");
        }
    }
}
