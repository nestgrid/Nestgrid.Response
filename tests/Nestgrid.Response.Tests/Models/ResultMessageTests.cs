using Shouldly;

namespace Nestgrid.Response.Tests.Models;

public sealed class ResultMessageTests
{
    [Fact]
    public void CreatedMessage_ShouldAssignProperties()
    {
        // Arrange
        const string message = "Username is required";
        const string code = "username_required";
        const string property = "Username";

        // Act
        var resultMessage = ResultMessagesFactory.Error(message, code, property);

        // Assert
        resultMessage.Message.ShouldBe(message);
        resultMessage.Code.ShouldBe(code);
        resultMessage.Property.ShouldBe(property);
        resultMessage.Severity.ShouldBe(ResultMessageSeverity.Error);
    }

    [Fact]
    public void CreatedMessage_WhenMessageIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        string message = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => ResultMessagesFactory.Error(message));

        // Assert
        exception.ParamName.ShouldBe(nameof(message));
    }
}
