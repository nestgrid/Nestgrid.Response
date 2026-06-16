using Shouldly;

namespace Nestgrid.Response.Tests.Results;

public sealed class AcceptedTests
{
    [Fact]
    public void Accepted_ShouldCreateAcceptedResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Accepted();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Accepted);
    }

    [Fact]
    public void Accepted_WithMessages_ShouldCreateAcceptedResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Info("Request accepted");

        // Act
        var result = ResultsFactory.Accepted(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Accepted, message);
    }

    [Fact]
    public void Accepted_WithValue_ShouldCreateAcceptedResultWithValue()
    {
        // Arrange
        const string value = "request";

        // Act
        var result = ResultsFactory.Accepted(value);

        // Assert
        FactoryAssertions.ShouldHaveValue(result, ResultStatus.Accepted, value);
    }

    [Fact]
    public void Accepted_WithValueAndMessages_ShouldCreateAcceptedResultWithValueAndMessages()
    {
        // Arrange
        const string value = "request";
        var message = ResultMessagesFactory.Info("Request accepted");

        // Act
        var result = ResultsFactory.Accepted(value, message);

        // Assert
        FactoryAssertions.ShouldHaveValueAndMessage(result, ResultStatus.Accepted, value, message);
    }

    [Fact]
    public void Accepted_ShouldNotHaveStringMessageOverload()
    {
        // Arrange
        const string methodName = nameof(ResultsFactory.Accepted);

        // Act
        var action = () => FactoryAssertions.ShouldNotHaveStringOverload(methodName);

        // Assert
        Should.NotThrow(action);
    }
}
