using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Constants;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userManager.Users
                .Select(u => new { u.Id, u.UserName, u.Email })
                .ToListAsync();

            _logger.LogInformation("Admin {Admin} a listé {Count} utilisateurs",
                User.Identity?.Name, users.Count);

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (!User.IsInRole(AppRoles.Admin) && currentUserId != id)
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"Utilisateur {id} introuvable" });

            return Ok(new { user.Id, user.UserName, user.Email });
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] RegisterUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(model.Role) || !AppRoles.AllowedRoles.Contains(model.Role))
                return BadRequest(new { message = $"Rôle '{model.Role}' non autorisé." });

            var user = new IdentityUser { UserName = model.UserName, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, model.Role);

            _logger.LogInformation("Admin {Admin} a créé l’utilisateur {UserName}",
                User.Identity?.Name, user.UserName);

            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                new { user.Id, user.UserName, user.Email, model.Role });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto model)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (!User.IsInRole(AppRoles.Admin) && currentUserId != id)
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"Utilisateur {id} introuvable" });

            user.UserName = model.UserName ?? user.UserName;
            user.Email = model.Email ?? user.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            _logger.LogInformation("Utilisateur {User} mis à jour par {Actor}",
                user.UserName, User.Identity?.Name);

            return Ok(new { user.Id, user.UserName, user.Email });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"Utilisateur {id} introuvable" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            _logger.LogWarning("Admin {Admin} a supprimé l’utilisateur {UserId}",
                User.Identity?.Name, id);

            return NoContent();
        }
    }

    public class RegisterUserDto
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Role { get; set; }
    }

    public class UpdateUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
