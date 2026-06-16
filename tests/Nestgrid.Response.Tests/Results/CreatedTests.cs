using Shouldly;

namespace Nestgrid.Response.Tests.Results;

public sealed class CreatedTests
{
    [Fact]
    public void Created_ShouldCreateCreatedResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Created();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Created);
    }

    [Fact]
    public void Created_WithMessages_ShouldCreateCreatedResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Info("Account created");

        // Act
        var result = ResultsFactory.Created(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Created, message);
    }

    [Fact]
    public void Created_WithValue_ShouldCreateCreatedResultWithValue()
    {
        // Arrange
        const int value = 42;

        // Act
        var result = ResultsFactory.Created(value);

        // Assert
        FactoryAssertions.ShouldHaveValue(result, ResultStatus.Created, value);
    }

    [Fact]
    public void Created_WithValueAndMessages_ShouldCreateCreatedResultWithValueAndMessages()
    {
        // Arrange
        const int value = 42;
        var message = ResultMessagesFactory.Info("Account created");

        // Act
        var result = ResultsFactory.Created(value, message);

        // Assert
        FactoryAssertions.ShouldHaveValueAndMessage(result, ResultStatus.Created, value, message);
    }

    [Fact]
    public void Created_ShouldNotHaveStringMessageOverload()
    {
        // Arrange
        const string methodName = nameof(ResultsFactory.Created);

        // Act
        var action = () => FactoryAssertions.ShouldNotHaveStringOverload(methodName);

        // Assert
        Should.NotThrow(action);
    }
}
