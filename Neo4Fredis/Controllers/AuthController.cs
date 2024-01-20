using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Neo4Fredis.Models.Entities;
using Neo4Fredis.Services.Implementation;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Neo4Fredis.Services.Implementation;

namespace Neo4Fredis.Controllers
{
    
    
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService authService;
        public AuthController(ILogger<AuthController> _logger, IAuthService authService)
        {
            this._logger = _logger;
            this.authService = authService;
        }

       
        [HttpPost]
        public IActionResult Add(string zaint, string mesto, MestoZaIzlaske model)
        {

            var objavljenoMestoResultSuccess = this.authService.Add(zaint, mesto, model);

            if (objavljenoMestoResultSuccess)
            {
                return RedirectToAction("GetAll", "MestoZaIzlaske");
            }
            return View();
        }
        public IActionResult Index1()
        {
            return View();
        }

        public IActionResult ObjaviMestoPage()
        {
            return View();
        }

        public IActionResult PrijaviSe()
        {
            return View();
        }
    }
}
