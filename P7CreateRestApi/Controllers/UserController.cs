using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Constants;
using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET ALL USERS (ADMIN ONLY)
        [HttpGet]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userManager.Users
                .Select(u => new { u.Id, u.Email, u.UserName })
                .ToListAsync();

            _logger.LogInformation("Admin {Admin} a listé {Count} utilisateurs", User.Identity?.Name, users.Count);

            return Ok(users);
        }

        // GET by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (!User.IsInRole(AppRoles.Admin) && currentUserId != id)
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"Utilisateur {id} introuvable" });

            return Ok(new { user.Id, user.Email });
        }

        // CREATE USER (ADMIN ONLY)
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] RegisterUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(model.Role) || !AppRoles.AllowedRoles.Contains(model.Role))
                return BadRequest(new { message = $"Rôle '{model.Role}' non autorisé." });

            var user = new AppUser
            {
                Email = model.Email,
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, model.Role);

            _logger.LogInformation("Admin {Admin} a créé l’utilisateur {Email}", User.Identity?.Name, user.Email);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { user.Id, user.Email, model.Role });
        }

        // UPDATE USER
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto model)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (!User.IsInRole(AppRoles.Admin) && currentUserId != id)
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"Utilisateur {id} introuvable" });

            if (model.Email != null)
            {
                user.Email = model.Email;
                user.UserName = model.UserName; 
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            _logger.LogInformation("Utilisateur {User} mis à jour par {Actor}", user.Email, User.Identity?.Name);

            return Ok(new { user.Id, user.Email });
        }

        // DELETE USER
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

            _logger.LogWarning("Admin {Admin} a supprimé l’utilisateur {UserId}", User.Identity?.Name, id);

            return NoContent();
        }
    }

    // DTOs

    public class RegisterUserDto
    {
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Role { get; set; }
    }


    public class UpdateUserDto
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
    }

}

