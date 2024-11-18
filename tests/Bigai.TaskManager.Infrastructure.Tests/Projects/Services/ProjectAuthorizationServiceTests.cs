using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Tests.Helpers;
using Bigai.TaskManager.Infrastructure.Projects.Services;

using FluentAssertions;

namespace Bigai.TaskManager.Infrastructure.Tests.Projects.Services;

public class ProjectAuthorizationServiceTests
{
    [Fact]
    public void Authorize_WhenThereIsNoPendingStatus_ReturnTrue()
    {
        // Arrange
        int amountProjects = 1;
        int userId = 1001;
        ProjectAuthorizationService projectAuthorizationService = new();
        ResourceOperation resourceOperation = ResourceOperation.Remove;
        Project project = ProjectHelper.GetProjects(amountProjects, userId).First();
        foreach (var workUnit in project.WorkUnits)
        {
            workUnit.ChangeStatus(Status.Completed);
        }

        // Act
        var authorized = projectAuthorizationService.Authorize(project, resourceOperation);

        // Assert
        authorized.Should().Be(true);
    }

    [Fact]
    public void Authorize_WhenThereIsPendingStatus_ReturnFalse()
    {
        // Arrange
        int amountProjects = 1;
        int userId = 1001;
        ProjectAuthorizationService projectAuthorizationService = new();
        ResourceOperation resourceOperation = ResourceOperation.Remove;
        Project project = ProjectHelper.GetProjects(amountProjects, userId).First();
        foreach (var workUnit in project.WorkUnits)
        {
            workUnit.ChangeStatus(Status.Pending);
        }

        // Act
        var authorized = projectAuthorizationService.Authorize(project, resourceOperation);

        // Assert
        authorized.Should().Be(false);
    }

    [Fact]
    public void Authorize_WhenProjectIsNull_ReturnFalse()
    {
        // Arrange
        ProjectAuthorizationService projectAuthorizationService = new();
        ResourceOperation resourceOperation = ResourceOperation.Remove;
        Project? project = null;

        // Act
        var authorized = projectAuthorizationService.Authorize(project, resourceOperation);

        // Assert
        authorized.Should().Be(false);
    }


    [Fact]
    public void AuthorizeLimit_WhenNumberTasksWithinLimit_ReturnTrue()
    {
        // Arrange
        int amountProjects = 1;
        int userId = 1001;
        ProjectAuthorizationService projectAuthorizationService = new();
        Project project = ProjectHelper.GetProjects(amountProjects, userId).First();

        // Act
        var authorized = projectAuthorizationService.AuthorizeLimit(project);

        // Assert
        authorized.Should().Be(true);
    }

    [Fact]
    public void AuthorizeLimit_WhenNumberTasksAboveLimit_ReturnFalse()
    {
        // Arrange
        int amountProjects = 1;
        int userId = 1001;
        ProjectAuthorizationService projectAuthorizationService = new();
        Project project = ProjectHelper.GetProjects(amountProjects, userId).First();

        // Act
        var authorized = projectAuthorizationService.AuthorizeLimit(project, 3);

        // Assert
        authorized.Should().Be(false);
    }

}