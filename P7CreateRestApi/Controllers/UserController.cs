using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // tout le monde doit être connecté
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

        // 🔹 GET: api/users
        // ✅ Admin : liste tous les utilisateurs
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email
                })
                .ToListAsync();

            _logger.LogInformation("Admin {Admin} a listé {Count} utilisateurs",
                User.Identity?.Name, users.Count);

            return Ok(users);
        }

        // 🔹 GET: api/users/{id}
        // ✅ Admin : peut voir n’importe qui
        // ✅ User : peut voir uniquement son propre profil
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Vérifie si l’utilisateur est admin ou accède à son propre compte
            if (!User.IsInRole("Admin") && currentUserId != id)
                return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"Utilisateur {id} introuvable" });

            return Ok(new { user.Id, user.UserName, user.Email });
        }

        // 🔹 POST: api/users
        // ✅ Admin : crée un nouvel utilisateur (avec rôle)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] RegisterUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Ajouter un rôle si spécifié
            if (!string.IsNullOrEmpty(model.Role))
            {
                if (!await _roleManager.RoleExistsAsync(model.Role))
                    await _roleManager.CreateAsync(new IdentityRole(model.Role));

                await _userManager.AddToRoleAsync(user, model.Role);
            }

            _logger.LogInformation("Admin {Admin} a créé l’utilisateur {UserName}",
                User.Identity?.Name, user.UserName);

            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                new { user.Id, user.UserName, user.Email });
        }

        // 🔹 PUT: api/users/{id}
        // ✅ Admin : peut modifier n’importe qui
        // ✅ User : peut modifier uniquement son propre compte
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto model)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Vérifie si l’utilisateur est admin ou édite son propre compte
            if (!User.IsInRole("Admin") && currentUserId != id)
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

        // 🔹 DELETE: api/users/{id}
        // ✅ Admin uniquement
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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

    // 🔸 DTOs
    public class RegisterUserDto
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Role { get; set; } // optionnel : "User" ou "Admin"
    }

    public class UpdateUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
