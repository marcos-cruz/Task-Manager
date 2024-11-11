using Bigai.TaskManager.Domain.Projects.Enums;

using FluentAssertions;

namespace Bigai.TaskManager.Domain.Tests.Projects.Models;

public class TaskTests
{
    [Fact]
    public void Create_Return_Instance_Task()
    {
        // arrange
        Domain.Projects.Models.Task task;
        var title = "task";
        var description = "description";
        var dueDate = new DateTimeOffset().AddDays(15);

        // act
        task = Domain.Projects.Models.Task.Create(title, description, dueDate);

        // assert
        task.Should().NotBeNull();
        task.Title.Should().Be(title);
        task.Description.Should().Be(description);
        task.DueDate.Should().Be(dueDate);
        task.Status.Should().Be(Status.Pending);
    }

}