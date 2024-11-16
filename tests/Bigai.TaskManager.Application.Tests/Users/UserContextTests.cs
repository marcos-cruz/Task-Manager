using System.Security.Claims;

using Bigai.TaskManager.Application.Users;
using Bigai.TaskManager.Domain.Projects.Constants;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

namespace Bigai.TaskManager.Application.Tests.Users;

public class UserContextTests
{
    [Fact]
    public void GetCurrentUser_WithAuthenticatedUser_ShouldReturnCurrentUser()
    {
        // arrange
        int userId = 101;
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Role, TaskManagerRoles.Manager),
            new(ClaimTypes.Role, TaskManagerRoles.User),
            new("UserId", $"{userId}"),
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

        httpContextAccessorMock.Setup(x => x.HttpContext)
                               .Returns(new DefaultHttpContext()
                               {
                                   User = user,
                               });

        var userContext = new UserContext(httpContextAccessorMock.Object);

        // act
        var currentUser = userContext.GetCurrentUser();

        // assert
        currentUser.Should().NotBeNull();
        currentUser!.UserId.Should().Be(userId);
        currentUser.Roles.Should().ContainInOrder(TaskManagerRoles.Manager, TaskManagerRoles.User);
    }

    [Fact]
    public void GetCurrentUser_WithUserContextNotPresent_ThrowsInvalidOperationException()
    {
        // arrange
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext)
                               .Returns((HttpContext)null);

        var userContext = new UserContext(httpContextAccessorMock.Object);

        // act
        Action action = () => userContext.GetCurrentUser();

        // assert
        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("User context is no present");
    }
}