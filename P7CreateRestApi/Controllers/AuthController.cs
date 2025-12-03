using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Services;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<AppUser> _signInManager;


        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            JwtService jwtService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                _logger.LogWarning("Tentative de connexion avec champs vides (IP : {IP})",
                    HttpContext.Connection.RemoteIpAddress?.ToString());
                return BadRequest("L'email et le mot de passe sont requis.");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                _logger.LogWarning(
                    "Échec connexion : email {Email} inexistant (IP : {IP})",
                    model.Email,
                    HttpContext.Connection.RemoteIpAddress?.ToString()
                );

                return Unauthorized("Email ou mot de passe invalide.");
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!validPassword)
            {
                _logger.LogWarning(
                    "Échec connexion : mauvais mot de passe pour {Email} (IP : {IP})",
                    model.Email,
                    HttpContext.Connection.RemoteIpAddress?.ToString()
                );

                return Unauthorized("Email ou mot de passe invalide.");
            }

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation(
                "Connexion réussie pour {Email} à {Time} (IP : {IP})",
                user.Email,
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            return Ok(new { token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();

            _logger.LogInformation("Déconnexion effectuée à {Time} depuis IP: {IP}",
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                HttpContext.Connection.RemoteIpAddress);

            return Ok(new { message = "Déconnexion réussie." });
        }
    
}
}