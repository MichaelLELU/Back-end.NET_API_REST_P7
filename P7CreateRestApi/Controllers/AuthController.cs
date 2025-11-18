using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Services;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<IdentityUser> userManager,
            JwtService jwtService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
            {
                _logger.LogWarning("Tentative de connexion avec des champs vides (IP : {IP})",
                    HttpContext.Connection.RemoteIpAddress?.ToString());
                return BadRequest("Le nom d'utilisateur et le mot de passe sont requis.");
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                _logger.LogWarning("Échec de connexion : utilisateur {User} inexistant (IP : {IP})",
                    model.UserName, HttpContext.Connection.RemoteIpAddress?.ToString());
                return Unauthorized("Nom d'utilisateur ou mot de passe invalide.");
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!validPassword)
            {
                _logger.LogWarning("Échec de connexion : mot de passe incorrect pour {User} (IP : {IP})",
                    model.UserName, HttpContext.Connection.RemoteIpAddress?.ToString());
                return Unauthorized("Nom d'utilisateur ou mot de passe invalide.");
            }

            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("✅ Connexion réussie pour {User} à {Time} (IP : {IP})",
                model.UserName,
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                HttpContext.Connection.RemoteIpAddress?.ToString());

            return Ok(new { token });
        }
    }

    public class LoginModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
