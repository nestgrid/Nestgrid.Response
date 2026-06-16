using Shouldly;

namespace Nestgrid.Response.Tests.Models;

public sealed class ResultTTests
{
    [Fact]
    public void CreatedResult_ShouldAssignValue()
    {
        // Arrange
        var value = new User("Ada");

        // Act
        var result = ResultsFactory.Ok(value);

        // Assert
        result.Value.ShouldBe(value);
    }

    [Fact]
    public void CreatedResult_ShouldAllowNullValue()
    {
        // Arrange
        User? value = null;

        // Act
        var result = ResultsFactory.Ok(value);

        // Assert
        result.Value.ShouldBeNull();
    }

    [Fact]
    public void CreatedResult_ShouldAssignStatus()
    {
        // Arrange
        const string message = "Missing";

        // Act
        var result = ResultsFactory.NotFound<User>(message);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public void CreatedResult_ShouldExposeMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Missing");

        // Act
        var result = ResultsFactory.NotFound<User>(message);

        // Assert
        result.Messages.ShouldBe([message]);
    }

    [Fact]
    public void CreatedResult_ShouldInheritFromResult()
    {
        // Arrange
        var value = new User("Ada");

        // Act
        var result = ResultsFactory.Ok(value);

        // Assert
        result.ShouldBeAssignableTo<Result>();
    }

    private sealed record User(string Name);
}
