namespace P7CreateRestApi.Dto.User
{
    public class RegisterUserDto
    {
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Role { get; set; }
    }
}
