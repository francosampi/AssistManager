using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using AsistManager.Models.ViewModels;

namespace AsistManager.Controllers
{
    public class AccessController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccessController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Redirigir al Home si está logeado. Si no, ir al login
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if(claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Evento");
            }

            return View();
        }

        //Logearse si las credenciales son correctas, si no, ir al login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Obtener las credenciales del archivo de configuración
            string userName = _configuration["AppSettings:User"];
            string password = _configuration["AppSettings:Password"];

            if (model.User == userName && model.Password == password)
            {
                List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, model.User),
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = model.KeepLoggedIn
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);

                return RedirectToAction("Index", "Evento");
            }

            TempData["AlertaTipo"] = "danger";
            TempData["AlertaMensaje"] = "Usuario no encontrado o contraseña incorrecta.";

            return View();
        }
    }
}
