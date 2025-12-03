using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Services;
using P7CreateRestApi.Test.Helpers;
using System.Net;
using Xunit;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsValid()
    {
        // Mock UserManager
        var um = UserManagerMockHelper.CreateUserManagerMock<AppUser>();

        // Mock SignInManager (not used in login, but required in constructor)
        var sm = UserManagerMockHelper.CreateSignInManagerMock(um.Object).Object;

        // Mock configuration for JWT
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "VerySecureKeyVerySecureKey12345" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            })
            .Build();

        var jwt = new JwtService(config, um.Object);

        // Fake user
        var user = new AppUser
        {
            Id = "1",
            Email = "test@test.com",
            UserName = "test"
        };

        // Essential mocks
        um.Setup(m => m.FindByEmailAsync("test@test.com")).ReturnsAsync(user);
        um.Setup(m => m.CheckPasswordAsync(user, "Password123!")).ReturnsAsync(true);
        um.Setup(m => m.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync(new List<string> { "Admin" });

        // Create controller
        var controller = new AuthController(
            um.Object,
            sm,
            jwt,
            NullLogger<AuthController>.Instance
        );

        // Add HttpContext manually (required for RemoteIpAddress)
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

        var dto = new LoginModel
        {
            Email = "test@test.com",
            Password = "Password123!"
        };

        // Act
        var result = await controller.Login(dto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var token = result.Value?.GetType().GetProperty("token")?.GetValue(result.Value);
        Assert.NotNull(token);
        Assert.IsType<string>(token);
        Assert.True(((string)token).Length > 10);
    }
}
