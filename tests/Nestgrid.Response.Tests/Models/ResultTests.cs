using System.Collections;
using Shouldly;

namespace Nestgrid.Response.Tests.Models;

public sealed class ResultTests
{
    [Fact]
    public void CreatedResult_ShouldAssignStatus()
    {
        // Arrange
        const ResultStatus status = ResultStatus.Conflict;

        // Act
        var result = new TestResultDouble(status);

        // Assert
        result.Status.ShouldBe(status);
    }

    [Fact]
    public void CreatedResult_ShouldExposeMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Warning("Check state");

        // Act
        var result = new TestResultDouble(ResultStatus.Failed, message);

        // Assert
        result.Messages.ShouldBe([message]);
    }

    [Fact]
    public void CreatedResult_ShouldExposeEmptyMessageCollectionWhenNoMessagesAreProvided()
    {
        // Arrange
        const ResultStatus status = ResultStatus.Ok;

        // Act
        var result = new TestResultDouble(status);

        // Assert
        result.Messages.ShouldBeEmpty();
    }

    [Fact]
    public void CreatedResult_ShouldExposeEmptyMessageCollectionWhenMessageArrayIsNull()
    {
        // Arrange
        ResultMessage[] messages = null!;

        // Act
        var result = new TestResultDouble(ResultStatus.Ok, messages);

        // Assert
        result.Messages.ShouldBeEmpty();
    }

    [Fact]
    public void Messages_ShouldBeImmutableSnapshot()
    {
        // Arrange
        var original = ResultMessagesFactory.Info("Original");
        var replacement = ResultMessagesFactory.Info("Replacement");
        var messages = new[] { original };

        // Act
        var result = new TestResultDouble(ResultStatus.Ok, messages);
        messages[0] = replacement;

        // Assert
        result.Messages.ShouldBe([original]);
    }

    [Fact]
    public void Messages_ShouldNotExposeMutableCollection()
    {
        // Arrange
        var message = ResultMessagesFactory.Info("Original");
        var result = new TestResultDouble(ResultStatus.Ok, message);
        var mutableView = (IList)result.Messages;

        // Act
        var exception = Should.Throw<NotSupportedException>(() => mutableView.Add(ResultMessagesFactory.Info("New")));

        // Assert
        exception.ShouldNotBeNull();
        result.Messages.ShouldBe([message]);
    }

    private sealed class TestResultDouble : Result
    {
        public TestResultDouble(ResultStatus status, params ResultMessage[] messages)
            : base(status, messages)
        {
        }
    }
}
