using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserController(UserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        // --- GET ALL ---
        [HttpGet("list")]
        public IActionResult GetAll()
        {
            var users = _userRepository.GetAll().ToList();
            return Ok(users);
        }

        // --- GET BY ID ---
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepository.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");
            return Ok(user);
        }

        // --- POST: Create ---
        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] CreateUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                Role = model.Role
            };

            var result = await _userRepository.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User created successfully");
        }

        // --- PUT: Update ---
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserModel model)
        {
            var user = await _userRepository.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;

            var result = await _userRepository.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User updated successfully");
        }

        // --- DELETE ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            var result = await _userRepository.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User deleted successfully");
        }
    }

    // --- DTOs ---
    public class CreateUserModel
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class UpdateUserModel
    {
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
