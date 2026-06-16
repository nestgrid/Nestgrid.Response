using Shouldly;

namespace Nestgrid.Response.Tests.ResultMessages;

public sealed class WarningTests
{
    [Fact]
    public void Warning_ShouldPopulateMessage()
    {
        // Arrange
        const string message = "License expires soon";
        const string code = "license_expiring";
        const string property = "License";

        // Act
        var result = ResultMessagesFactory.Warning(message, code, property);

        // Assert
        result.Message.ShouldBe(message);
        result.Code.ShouldBe(code);
        result.Property.ShouldBe(property);
        result.Severity.ShouldBe(ResultMessageSeverity.Warning);
    }

    [Fact]
    public void Warning_WhenMessageIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        string message = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => ResultMessagesFactory.Warning(message));

        // Assert
        exception.ParamName.ShouldBe(nameof(message));
    }
}
