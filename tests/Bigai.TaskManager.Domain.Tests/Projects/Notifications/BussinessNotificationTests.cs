using Bigai.TaskManager.Domain.Projects.Notifications;

using FluentAssertions;

namespace Bigai.TaskManager.Domain.Tests.Projects.Notifications;

public class BussinessNotificationTests
{
    [Fact]
    public void BussinessNotification_Return_Instance_BussinessNotification()
    {
        // arrange
        BussinessNotification notification;
        string code = "Code";
        string message = "Message";

        // act
        notification = new BussinessNotification(code, message);

        // assert
        notification.Should().NotBeNull();
        notification.Code.Should().Be(code);
        notification.Message.Should().Be(message);
    }
}