using Bigai.TaskManager.Application.Users;
using Bigai.TaskManager.Domain.Projects.Constants;

using FluentAssertions;

namespace Bigai.TaskManager.Application.Tests.Users;

public class CurrentUserTests
{
    [Fact]
    public void IsInRole_WithMatchingRole_ShouldReturnTrue()
    {
        // Arrange
        int userId = 101;
        var currentUser = new CurrentUser(userId, [TaskManagerRoles.Manager, TaskManagerRoles.User]);

        // Act
        var isInRole = currentUser.IsInRole(TaskManagerRoles.Manager);

        // Assert
        isInRole.Should().BeTrue();
    }

    [Fact]
    public void IsInRole_WithMatchingRole_ShouldReturnFalse()
    {
        // Arrange
        int userId = 101;
        var currentUser = new CurrentUser(userId, [TaskManagerRoles.User]);

        // Act
        var isInRole = currentUser.IsInRole(TaskManagerRoles.Manager);

        // Assert
        isInRole.Should().BeFalse();
    }

    [Fact]
    public void IsInRole_WithNoMatchingRole_ShouldReturnFalse()
    {
        // Arrange
        int userId = 101;
        var currentUser = new CurrentUser(userId, [TaskManagerRoles.Manager, TaskManagerRoles.User]);

        // Act
        var isInRole = currentUser.IsInRole(TaskManagerRoles.Manager.ToLower());

        // Assert
        isInRole.Should().BeFalse();
    }
}