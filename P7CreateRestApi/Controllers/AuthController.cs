using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Dto.Login;
using P7CreateRestApi.Services;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JwtService _jwtService;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return UnauthorizedProblem();

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                lockoutOnFailure: true
            );

            if (!result.Succeeded)
                return UnauthorizedProblem();

            var response = new LoginResponseDto
            {
                Token = _jwtService.GenerateToken(user),
                Expiration = DateTime.UtcNow.AddHours(2)
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Déconnexion réussie." });
        }

        private IActionResult UnauthorizedProblem()
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = "Identifiants invalides.",
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }
}
