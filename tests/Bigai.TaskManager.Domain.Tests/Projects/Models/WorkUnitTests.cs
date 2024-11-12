using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;

using FluentAssertions;

namespace Bigai.TaskManager.Domain.Tests.Projects.Models;

public class WorkUnitTests
{
    [Fact]
    public void Create_Return_Instance_Task()
    {
        // arrange
        WorkUnit task;
        var title = "task";
        var description = "description";
        var dueDate = new DateTimeOffset().AddDays(15);
        var priority = Priority.High;

        // act
        task = WorkUnit.Create(title, description, dueDate, priority);

        // assert
        task.Should().NotBeNull();
        task.Title.Should().Be(title);
        task.Description.Should().Be(description);
        task.DueDate.Should().Be(dueDate);
        task.Status.Should().Be(Status.Pending);
    }

    [Fact]
    public void AssignToUser_MustAssingWorkUnitToUser()
    {
        // arrange
        var userId = 1001;
        var dueDate = new DateTimeOffset().AddDays(15);
        WorkUnit task = WorkUnit.Create("Work unit title", "Work unit description", dueDate, Priority.High);

        // act
        task.AssignToUser(userId);

        // assert
        task.Should().NotBeNull();
        task.UserId.Should().Be(userId);
    }

    [Fact]
    public void AssignToProject_MustAssingWorkUnitToProject()
    {
        // arrange
        var projectId = 1001;
        var dueDate = new DateTimeOffset().AddDays(15);
        WorkUnit task = WorkUnit.Create("Work unit title", "Work unit description", dueDate, Priority.High);

        // act
        task.AssignToProject(projectId);

        // assert
        task.Should().NotBeNull();
        task.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public void ChangeStatus_MustChangeStatus()
    {
        // arrange
        var dueDate = new DateTimeOffset().AddDays(15);
        WorkUnit task = WorkUnit.Create("Work unit title", "Work unit description", dueDate, Priority.High);

        // act
        task.ChangeStatus(Status.InProgress);

        // assert
        task.Status.Should().Be(Status.InProgress);
    }
}