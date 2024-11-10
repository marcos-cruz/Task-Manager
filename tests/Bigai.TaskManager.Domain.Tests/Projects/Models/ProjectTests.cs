using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;

using FluentAssertions;

namespace Bigai.TaskManager.Domain.Tests.Projects.Models;

public class ProjectTests
{
    [Fact]
    public void Create_Return_Instance_Project()
    {
        // arrange
        Project project;
        int userId = 101;
        string name = "test project";
        Priority priority = Priority.High;

        // act
        project = Project.Create(userId, name, priority);

        // assert
        project.Should().NotBeNull();
        project.UserId.Should().Be(userId);
        project.Name.Should().Be(name);
        project.Priority.Should().Be(priority);
        project.Tasks.Should().NotBeNull();
        project.Tasks.Should().BeEmpty();
    }
}