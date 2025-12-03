using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace P7CreateRestApi.Test.Helpers
{
    public static class UserManagerMockHelper
    {
        public static Mock<UserManager<TUser>> CreateUserManagerMock<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();

            return new Mock<UserManager<TUser>>(
                store.Object,
                null, null, null, null, null, null, null, null
            );
        }

        public static Mock<RoleManager<IdentityRole>> CreateRoleManagerMock()
        {
            return new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object,
                null, null, null, null
            );
        }

        public static Mock<SignInManager<TUser>> CreateSignInManagerMock<TUser>(UserManager<TUser> userManager)
            where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();

            return new Mock<SignInManager<TUser>>(
                userManager,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null, null, null
            );
        }
    }
}
