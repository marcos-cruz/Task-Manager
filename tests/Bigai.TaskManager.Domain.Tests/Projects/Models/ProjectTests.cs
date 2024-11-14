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
        string name = "test project";

        // act
        project = Project.Create(name);

        // assert
        project.Should().NotBeNull();
        project.Name.Should().Be(name);
        project.WorkUnits.Should().NotBeNull();
        project.WorkUnits.Should().BeEmpty();
    }

    [Fact]
    public void AddWorkUnit_MustIncludeWorkUnit()
    {
        // arrange
        Project project = Project.Create("test project");
        WorkUnit workUnit = WorkUnit.Create("task title", "Task description", DateTime.Now.AddDays(15), Priority.Low);

        // act
        project.AddWorkUnit(workUnit);

        // assert
        project.WorkUnits.Should().NotBeNull();
        project.WorkUnits.First().Should().Be(workUnit);
    }
}