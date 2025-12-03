using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using P7CreateRestApi.Constants;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Test.Helpers;
using System.Security.Claims;
using Xunit;

public class UserControllerTests
{
    private UserController Create(Mock<UserManager<AppUser>> um, Mock<RoleManager<IdentityRole>> rm, ClaimsPrincipal user)
    {
        var controller = new UserController(
            um.Object,
            rm.Object,
            NullLogger<UserController>.Instance
        );

        controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        return controller;
    }

    // GET ALL
    [Fact]
    public async Task GetAll_ShouldReturnUsers_WhenAdmin()
    {
        var um = UserManagerMockHelper.CreateUserManagerMock<AppUser>();
        var rm = UserManagerMockHelper.CreateRoleManagerMock();

        var users = new List<AppUser>
        {
            new AppUser { Id = "1", Email = "admin@test.com" },
            new AppUser { Id = "2", Email = "user@test.com" }
        }.AsQueryable();

        um.Setup(u => u.Users).Returns(new TestAsyncEnumerable<AppUser>(users));

        var identity = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Role, AppRoles.Admin) }, "Auth"));

        var controller = Create(um, rm, identity);

        var result = await controller.GetAll() as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(2, ((IEnumerable<object>)result.Value).Count());
    }

    // GET BY ID (self)
    [Fact]
    public async Task GetById_ShouldReturnUser_WhenSelf()
    {
        var um = UserManagerMockHelper.CreateUserManagerMock<AppUser>();
        var rm = UserManagerMockHelper.CreateRoleManagerMock();

        var user = new AppUser { Id = "10", Email = "me@test.com" };

        um.Setup(u => u.FindByIdAsync("10")).ReturnsAsync(user);
        um.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("10");

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, "10") }, "Auth"));

        var controller = Create(um, rm, principal);

        var result = await controller.GetById("10") as OkObjectResult;

        Assert.NotNull(result);
    }

    // Forbidden GET
    [Fact]
    public async Task GetById_ShouldForbid_WhenDifferentUser()
    {
        var um = UserManagerMockHelper.CreateUserManagerMock<AppUser>();
        var rm = UserManagerMockHelper.CreateRoleManagerMock();

        um.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("111");

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, "111") }, "Auth"));

        var controller = Create(um, rm, principal);

        var result = await controller.GetById("222");

        Assert.IsType<ForbidResult>(result);
    }

    // CREATE admin only
    [Fact]
    public async Task Create_ShouldCreate_WhenAdmin()
    {
        var um = UserManagerMockHelper.CreateUserManagerMock<AppUser>();
        var rm = UserManagerMockHelper.CreateRoleManagerMock();

        um.Setup(m => m.CreateAsync(It.IsAny<AppUser>(), "Password123!"))
            .ReturnsAsync(IdentityResult.Success);

        um.Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), AppRoles.User))
            .ReturnsAsync(IdentityResult.Success);

        var dto = new RegisterUserDto
        {
            Email = "new@test.com",
            UserName = "new",
            Password = "Password123!",
            Role = AppRoles.User
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Role, AppRoles.Admin) }, "Auth"));

        var controller = Create(um, rm, principal);

        var result = await controller.Create(dto);

        Assert.IsType<CreatedAtActionResult>(result);
    }


    // UPDATE self
    [Fact]
    public async Task Update_ShouldUpdate_WhenSelf()
    {
        var um = UserManagerMockHelper.CreateUserManagerMock<AppUser>();
        var rm = UserManagerMockHelper.CreateRoleManagerMock();

        var user = new AppUser { Id = "20", Email = "old@test.com" };

        um.Setup(m => m.FindByIdAsync("20")).ReturnsAsync(user);
        um.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("20");
        um.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var dto = new UpdateUserDto { Email = "new@test.com" };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, "20") }, "Auth"));

        var controller = Create(um, rm, principal);

        var result = await controller.Update("20", dto);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal("new@test.com", user.Email);
    }

    // DELETE
    [Fact]
    public async Task Delete_ShouldDelete_WhenAdmin()
    {
        var um = UserManagerMockHelper.CreateUserManagerMock<AppUser>();
        var rm = UserManagerMockHelper.CreateRoleManagerMock();

        var user = new AppUser { Id = "50" };

        um.Setup(m => m.FindByIdAsync("50")).ReturnsAsync(user);
        um.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Role, AppRoles.Admin) }, "Auth"));

        var controller = Create(um, rm, principal);

        var result = await controller.Delete("50");

        Assert.IsType<NoContentResult>(result);
    }
}
