using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Tests.Helpers;
using Bigai.TaskManager.Infrastructure.Projects.Services;

using FluentAssertions;

namespace Bigai.TaskManager.Infrastructure.Tests.Projects.Services;

public class SerializeServiceTests
{
    [Fact]
    public void WorkUnitToJson_ReturnsJsonString()
    {
        // Arrange
        int amountProjects = 1;
        int userId = 1001;
        SerializeService serializeService = new();
        Project project = ProjectHelper.GetProjects(amountProjects, userId).First();
        WorkUnit workUnit = project.WorkUnits.ToArray()[0];

        // Act
        var json = serializeService.WorkUnitToJson(workUnit);

        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void JsonToWorkUnit_ReturnsInstanceOfWorkUnit()
    {
        // Arrange
        int amountProjects = 1;
        int userId = 1001;
        SerializeService serializeService = new();
        Project project = ProjectHelper.GetProjects(amountProjects, userId).First();
        WorkUnit workUnit = project.WorkUnits.ToArray()[0];
        var json = serializeService.WorkUnitToJson(workUnit);

        // Act
        var deserializedInstance = serializeService.JsonToWorkUnit(json);

        // Assert
        deserializedInstance.Should().NotBeNull();
        deserializedInstance!.Id.Should().Be(workUnit.Id);
        deserializedInstance.ProjectId.Should().Be(workUnit.ProjectId);
        deserializedInstance.UserId.Should().Be(workUnit.UserId);
        deserializedInstance.Title.Should().Be(workUnit.Title);
        deserializedInstance.Description.Should().Be(workUnit.Description);
        deserializedInstance.DueDate.Should().Be(workUnit.DueDate);
        deserializedInstance.Status.Should().Be(workUnit.Status);
        deserializedInstance.Priority.Should().Be(workUnit.Priority);
    }

}