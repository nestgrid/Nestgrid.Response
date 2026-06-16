using Shouldly;

namespace Nestgrid.Response.Tests.ResultMessages;

public sealed class InfoTests
{
    [Fact]
    public void Info_ShouldPopulateMessage()
    {
        // Arrange
        const string message = "Loaded from cache";
        const string code = "cache_hit";
        const string property = "User";

        // Act
        var result = ResultMessagesFactory.Info(message, code, property);

        // Assert
        result.Message.ShouldBe(message);
        result.Code.ShouldBe(code);
        result.Property.ShouldBe(property);
        result.Severity.ShouldBe(ResultMessageSeverity.Information);
    }

    [Fact]
    public void Info_WhenMessageIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        string message = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => ResultMessagesFactory.Info(message));

        // Assert
        exception.ParamName.ShouldBe(nameof(message));
    }
}
