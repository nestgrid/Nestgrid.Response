using Shouldly;

namespace Nestgrid.Response.Tests.ResultMessages;

public sealed class ErrorTests
{
    [Fact]
    public void Error_ShouldPopulateMessage()
    {
        // Arrange
        const string message = "User not found";
        const string code = "user_not_found";
        const string property = "UserId";

        // Act
        var result = ResultMessagesFactory.Error(message, code, property);

        // Assert
        result.Message.ShouldBe(message);
        result.Code.ShouldBe(code);
        result.Property.ShouldBe(property);
        result.Severity.ShouldBe(ResultMessageSeverity.Error);
    }

    [Fact]
    public void Error_WhenMessageIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        string message = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => ResultMessagesFactory.Error(message));

        // Assert
        exception.ParamName.ShouldBe(nameof(message));
    }
}
