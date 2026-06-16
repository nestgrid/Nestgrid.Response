using Shouldly;

namespace Nestgrid.Response.Tests.Results;

public sealed class OkTests
{
    [Fact]
    public void Ok_ShouldCreateOkResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Ok();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Ok);
    }

    [Fact]
    public void Ok_WithMessages_ShouldCreateOkResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Info("Loaded from cache");

        // Act
        var result = ResultsFactory.Ok(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Ok, message);
    }

    [Fact]
    public void Ok_WithValue_ShouldCreateOkResultWithValue()
    {
        // Arrange
        const string value = "user";

        // Act
        var result = ResultsFactory.Ok(value);

        // Assert
        FactoryAssertions.ShouldHaveValue(result, ResultStatus.Ok, value);
    }

    [Fact]
    public void Ok_WithValueAndMessages_ShouldCreateOkResultWithValueAndMessages()
    {
        // Arrange
        const string value = "user";
        var message = ResultMessagesFactory.Info("Loaded from cache");

        // Act
        var result = ResultsFactory.Ok(value, message);

        // Assert
        FactoryAssertions.ShouldHaveValueAndMessage(result, ResultStatus.Ok, value, message);
    }

    [Fact]
    public void Ok_ShouldNotHaveStringMessageOverload()
    {
        // Arrange
        const string methodName = nameof(ResultsFactory.Ok);

        // Act
        var action = () => FactoryAssertions.ShouldNotHaveStringOverload(methodName);

        // Assert
        Should.NotThrow(action);
    }
}
