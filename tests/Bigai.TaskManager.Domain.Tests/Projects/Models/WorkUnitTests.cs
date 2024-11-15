using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Services;

using FluentAssertions;

using Moq;

namespace Bigai.TaskManager.Domain.Tests.Projects.Models;

public class WorkUnitTests
{
    [Fact]
    public void Create_Return_InstanceWorkUnit()
    {
        // arrange
        WorkUnit workUnit;
        var title = "task";
        var description = "description";
        var dueDate = DateTime.Now.AddDays(15);
        var priority = Priority.High;

        // act
        workUnit = WorkUnit.Create(title, description, dueDate, priority);

        // assert
        workUnit.Should().NotBeNull();
        workUnit.Title.Should().Be(title);
        workUnit.Description.Should().Be(description);
        workUnit.DueDate.Should().Be(dueDate);
        workUnit.Status.Should().Be(Status.Pending);
    }

    [Fact]
    public void AssignToUser_MustAssingWorkUnitToUser()
    {
        // arrange
        var userId = 1001;
        var dueDate = DateTime.Now.AddDays(15);
        WorkUnit workUnit = WorkUnit.Create("Work unit title", "Work unit description", dueDate, Priority.High);

        // act
        workUnit.AssignToUser(userId);

        // assert
        workUnit.Should().NotBeNull();
        workUnit.UserId.Should().Be(userId);
    }

    [Fact]
    public void AssignToProject_MustAssingWorkUnitToProject()
    {
        // arrange
        var projectId = 1001;
        var dueDate = DateTime.Now.AddDays(15);
        WorkUnit workUnit = WorkUnit.Create("Work unit title", "Work unit description", dueDate, Priority.High);

        // act
        workUnit.AssignToProject(projectId);

        // assert
        workUnit.Should().NotBeNull();
        workUnit.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public void ChangeStatus_MustChangeStatus()
    {
        // arrange
        var dueDate = DateTime.Now.AddDays(15);
        WorkUnit workUnit = WorkUnit.Create("Work unit title", "Work unit description", dueDate, Priority.High);

        // act
        workUnit.ChangeStatus(Status.InProgress);

        // assert
        workUnit.Status.Should().Be(Status.InProgress);
    }

    [Fact]
    public void GetDelta_WhenChanges_ReturnsOnlyUpdatedData()
    {
        // arrange
        var updatedTitle = "Title of work unit";
        var updatedDescription = "Updated description";
        var updatedDueDate = DateTime.Now.AddDays(10);
        var updatedStatus = Status.InProgress;
        var workUnitExisting = WorkUnit.Create("Title of work unit", "Description of work unit", DateTime.Now.AddDays(15), Priority.High);
        var workUnitUpdating = WorkUnit.Create(updatedTitle, updatedDescription, updatedDueDate, Priority.High);
        workUnitUpdating.ChangeStatus(updatedStatus);

        // act
        var delta = workUnitExisting.GetDelta(workUnitUpdating);

        // assert
        delta.Should().NotBeNull();
        delta!.Title.Should().Be("");
        delta.Description.Should().Be(updatedDescription);
        delta.DueDate.Should().Be(updatedDueDate);
        delta.Status.Should().Be(updatedStatus);
    }

    [Fact]
    public void GetDelta_WhenNotChanges_ReturnsNull()
    {
        // arrange
        var updatedTitle = "Title of work unit";
        var updatedDescription = "Description of work unit";
        var updatedDueDate = DateTime.Now.AddDays(10);
        var workUnitExisting = WorkUnit.Create(updatedTitle, updatedDescription, updatedDueDate, Priority.High);
        var workUnitUpdating = WorkUnit.Create(updatedTitle, updatedDescription, updatedDueDate, Priority.High);

        // act
        var delta = workUnitExisting.GetDelta(workUnitUpdating);

        // assert
        delta.Should().BeNull();
    }

    [Fact]
    public void AddHistory_MustAddHistory()
    {
        // arrange
        var existingWorkUnit = WorkUnit.Create("Title of work unit", "Description of work unit", DateTime.Now.AddDays(15), Priority.High);
        var changeRequest = WorkUnit.Create("Updated title of work unit", "updated description of work unit", DateTime.Now.AddDays(10), Priority.High);
        changeRequest.ChangeStatus(Status.InProgress);


        var serializeServiceMock = new Mock<ISerializeService>();
        serializeServiceMock
            .Setup(repo => repo.WorkUnitToJson(It.IsAny<WorkUnit>()))
            .Returns(Guid.NewGuid().ToString());

        var changedValues = existingWorkUnit.GetDelta(changeRequest);
        History history = History.Create(existingWorkUnit, changedValues!, serializeServiceMock.Object);

        // act
        existingWorkUnit.AddHistory(history);

        // assert
        existingWorkUnit.Historys.First().Should().Be(history);
    }

}
