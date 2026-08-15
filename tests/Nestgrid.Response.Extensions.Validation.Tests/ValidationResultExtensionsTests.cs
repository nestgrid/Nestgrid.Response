using System.ComponentModel.DataAnnotations;
using Shouldly;

namespace Nestgrid.Response.Extensions.Validation.Tests;

public sealed class ValidationResultExtensionsTests
{
    [Fact]
    public void ToMessage_ShouldCopyMessage()
    {
        // Arrange
        var validationResult = new ValidationResult("Name is required", new[] { "Name" });

        // Act
        var message = validationResult.ToMessage();

        // Assert
        message.Message.ShouldBe("Name is required");
        message.Code.ShouldBeNull();
        message.Property.ShouldBeNull();
    }

    [Fact]
    public void ToMessage_ShouldDefaultSeverityToWarning()
    {
        // Arrange
        var validationResult = new ValidationResult("Name is required");

        // Act
        var message = validationResult.ToMessage();

        // Assert
        message.Severity.ShouldBe(ResultMessageSeverity.Warning);
    }

    [Fact]
    public void ToMessage_WithSeverityOverride_ShouldUseSeverity()
    {
        // Arrange
        var validationResult = new ValidationResult("Name is required");

        // Act
        var message = validationResult.ToMessage(ResultMessageSeverity.Error);

        // Assert
        message.Severity.ShouldBe(ResultMessageSeverity.Error);
    }

    [Fact]
    public void ToMessage_WhenSeverityIsUnsupported_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var validationResult = new ValidationResult("Name is required");
        var severity = (ResultMessageSeverity)999;

        // Act
        var exception = Should.Throw<ArgumentOutOfRangeException>(() => validationResult.ToMessage(severity));

        // Assert
        exception.ParamName.ShouldBe(nameof(severity));
        exception.Message.ShouldContain("Unsupported message severity.");
    }

    [Fact]
    public void ToMessage_WhenValidationResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ValidationResult validationResult = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => validationResult.ToMessage());

        // Assert
        exception.ParamName.ShouldBe(nameof(validationResult));
    }

    [Fact]
    public void ToMessages_WhenCollectionIsEmpty_ShouldReturnEmptyArray()
    {
        // Arrange
        var validationResults = Array.Empty<ValidationResult>();

        // Act
        var messages = validationResults.ToMessages();

        // Assert
        messages.ShouldBeEmpty();
    }

    [Fact]
    public void ToMessages_WithSingleItem_ShouldConvertMessage()
    {
        // Arrange
        var validationResults = new[]
        {
            new ValidationResult("Name is required")
        };

        // Act
        var messages = validationResults.ToMessages();

        // Assert
        messages.Length.ShouldBe(1);
        messages[0].Message.ShouldBe("Name is required");
    }

    [Fact]
    public void ToMessages_WithMultipleItems_ShouldPreserveOrdering()
    {
        // Arrange
        var validationResults = new[]
        {
            new ValidationResult("First"),
            new ValidationResult("Second"),
            new ValidationResult("Third")
        };

        // Act
        var messages = validationResults.ToMessages();

        // Assert
        messages.Select(message => message.Message).ShouldBe(new[] { "First", "Second", "Third" });
    }

    [Fact]
    public void ToMessages_WithSeverityOverride_ShouldPropagateSeverity()
    {
        // Arrange
        var validationResults = new[]
        {
            new ValidationResult("First"),
            new ValidationResult("Second")
        };

        // Act
        var messages = validationResults.ToMessages(ResultMessageSeverity.Information);

        // Assert
        messages.All(message => message.Severity == ResultMessageSeverity.Information).ShouldBeTrue();
    }

    [Fact]
    public void ToMessages_WhenCollectionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<ValidationResult> validationResults = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => validationResults.ToMessages());

        // Assert
        exception.ParamName.ShouldBe(nameof(validationResults));
    }

    [Fact]
    public void ToMessagesWithProperties_WithMultipleMembers_ShouldCreateMessagesInSourceOrder()
    {
        var validationResults = new[]
        {
            new ValidationResult("Invalid user", new[] { "Name", "Email" })
        };

        var messages = validationResults.ToMessagesWithProperties();

        messages.Select(message => (message.Property, message.Code, message.Message))
            .ShouldBe(new[]
            {
                ((string?)"Name", (string?)"validation_failed", "Invalid user"),
                ((string?)"Email", (string?)"validation_failed", "Invalid user")
            });
    }

    [Fact]
    public void ToMessagesWithProperties_ShouldIgnoreBlankMembersAndKeepMemberlessResult()
    {
        var validationResults = new[]
        {
            new ValidationResult("Invalid user", new[] { " ", "Name", "\t" }),
            new ValidationResult(null)
        };

        var messages = validationResults.ToMessagesWithProperties("custom", ResultMessageSeverity.Error);

        messages.Select(message => (message.Property, message.Code, message.Message, message.Severity))
            .ShouldBe(new[]
            {
                ((string?)"Name", (string?)"custom", "Invalid user", ResultMessageSeverity.Error),
                ((string?)null, (string?)"custom", "The entity is invalid.", ResultMessageSeverity.Error)
            });
    }

    [Fact]
    public void ToInvalidResultWithPropertiesGeneric_WithEmptyInput_ShouldReturnTypedInvalidResultWithoutMessages()
    {
        var result = Array.Empty<ValidationResult>().ToInvalidResultWithProperties<UserDto>();

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Value.ShouldBeNull();
        result.Messages.ShouldBeEmpty();
    }

    [Fact]
    public void ToMessagesWithProperties_WhenCollectionIsNull_ShouldThrowArgumentNullException()
    {
        IEnumerable<ValidationResult> validationResults = null!;

        var exception = Should.Throw<ArgumentNullException>(() => validationResults.ToMessagesWithProperties());

        exception.ParamName.ShouldBe(nameof(validationResults));
    }

    [Fact]
    public void ToInvalidResult_WithSingleValidationResult_ShouldReturnInvalidResult()
    {
        // Arrange
        var validationResult = new ValidationResult("Name is required");

        // Act
        var result = validationResult.ToInvalidResult();

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Messages.Single().Message.ShouldBe("Name is required");
        result.Messages.Single().Severity.ShouldBe(ResultMessageSeverity.Warning);
    }

    [Fact]
    public void ToInvalidResult_WithMultipleValidationResults_ShouldPreserveMessages()
    {
        // Arrange
        var validationResults = new[]
        {
            new ValidationResult("First"),
            new ValidationResult("Second")
        };

        // Act
        var result = validationResults.ToInvalidResult();

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Messages.Select(message => message.Message).ShouldBe(new[] { "First", "Second" });
    }

    [Fact]
    public void ToInvalidResult_WithSeverityOverride_ShouldPropagateSeverity()
    {
        // Arrange
        var validationResults = new[]
        {
            new ValidationResult("First"),
            new ValidationResult("Second")
        };

        // Act
        var result = validationResults.ToInvalidResult(ResultMessageSeverity.Error);

        // Assert
        result.Messages.All(message => message.Severity == ResultMessageSeverity.Error).ShouldBeTrue();
    }

    [Fact]
    public void ToInvalidResult_WhenCollectionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<ValidationResult> validationResults = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => validationResults.ToInvalidResult());

        // Assert
        exception.ParamName.ShouldBe(nameof(validationResults));
    }

    [Fact]
    public void ToInvalidResultGeneric_WithSingleValidationResult_ShouldReturnTypedInvalidResult()
    {
        // Arrange
        var validationResult = new ValidationResult("Name is required");

        // Act
        var result = validationResult.ToInvalidResult<UserDto>();

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Value.ShouldBeNull();
        result.Messages.Single().Message.ShouldBe("Name is required");
        result.Messages.Single().Severity.ShouldBe(ResultMessageSeverity.Warning);
    }

    [Fact]
    public void ToInvalidResultGeneric_WithMultipleValidationResults_ShouldPreserveMessages()
    {
        // Arrange
        var validationResults = new[]
        {
            new ValidationResult("First"),
            new ValidationResult("Second")
        };

        // Act
        var result = validationResults.ToInvalidResult<UserDto>();

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.Messages.Select(message => message.Message).ShouldBe(new[] { "First", "Second" });
    }

    [Fact]
    public void ToInvalidResultGeneric_WithSeverityOverride_ShouldPropagateSeverity()
    {
        // Arrange
        var validationResults = new[]
        {
            new ValidationResult("First"),
            new ValidationResult("Second")
        };

        // Act
        var result = validationResults.ToInvalidResult<UserDto>(ResultMessageSeverity.Information);

        // Assert
        result.Messages.All(message => message.Severity == ResultMessageSeverity.Information).ShouldBeTrue();
    }

    [Fact]
    public void ToInvalidResultGeneric_WhenCollectionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<ValidationResult> validationResults = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => validationResults.ToInvalidResult<UserDto>());

        // Assert
        exception.ParamName.ShouldBe(nameof(validationResults));
    }

    private sealed record UserDto(int Id, string Name);
}
