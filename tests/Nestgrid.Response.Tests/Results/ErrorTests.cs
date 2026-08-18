using Shouldly;

namespace Nestgrid.Response.Tests.Results;

public sealed class ErrorTests
{
    [Fact]
    public void Error_ShouldCreateErrorResult()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Error();

        // Assert
        FactoryAssertions.ShouldHaveStatus(result, ResultStatus.Error);
    }

    [Fact]
    public void Error_WithString_ShouldCreateErrorResultWithErrorMessage()
    {
        // Arrange
        const string message = "Unexpected failure";

        // Act
        var result = ResultsFactory.Error(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Error, message);
    }

    [Fact]
    public void Error_WithMessages_ShouldCreateErrorResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Unexpected failure");

        // Act
        var result = ResultsFactory.Error(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Error, message);
    }

    [Fact]
    public void Error_WithException_ShouldCreateSafeGenericErrorResult()
    {
        // Arrange
        var exception = new InvalidOperationException("Unexpected failure");

        // Act
        var result = ResultsFactory.Error(exception);

        // Assert
        result.Status.ShouldBe(ResultStatus.Error);
        result.Messages.Count.ShouldBe(1);
        result.Messages[0].Message.ShouldBe("An unexpected error occurred.");
        result.Messages[0].Code.ShouldBeNull();
        result.Messages[0].Severity.ShouldBe(ResultMessageSeverity.Error);
    }

    [Fact]
    public void ErrorWithDiagnosticDetails_WithException_ShouldPreserveExceptionMessageAndCode()
    {
        var exception = new InvalidOperationException("Unexpected failure");

        var result = ResultsFactory.ErrorWithDiagnosticDetails(exception);

        result.Status.ShouldBe(ResultStatus.Error);
        result.Messages.Single().Message.ShouldBe(exception.Message);
        result.Messages.Single().Code.ShouldBe(nameof(InvalidOperationException));
    }

    [Fact]
    public void Error_WithNullException_ShouldThrowArgumentNullException()
    {
        // Arrange
        Exception exception = null!;

        // Act
        var thrown = Should.Throw<ArgumentNullException>(() => ResultsFactory.Error(exception));

        // Assert
        thrown.ParamName.ShouldBe(nameof(exception));
    }

    [Fact]
    public void ErrorOfT_ShouldCreateErrorResultWithDefaultValue()
    {
        // Arrange

        // Act
        var result = ResultsFactory.Error<string>();

        // Assert
        FactoryAssertions.ShouldHaveDefaultValue(result, ResultStatus.Error);
    }

    [Fact]
    public void ErrorOfT_WithString_ShouldCreateErrorResultWithErrorMessage()
    {
        // Arrange
        const string message = "Unexpected failure";

        // Act
        var result = ResultsFactory.Error<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveErrorMessage(result, ResultStatus.Error, message);
    }

    [Fact]
    public void ErrorOfT_WithMessages_ShouldCreateErrorResultWithMessages()
    {
        // Arrange
        var message = ResultMessagesFactory.Error("Unexpected failure");

        // Act
        var result = ResultsFactory.Error<string>(message);

        // Assert
        FactoryAssertions.ShouldHaveMessage(result, ResultStatus.Error, message);
    }

    [Fact]
    public void ErrorOfT_WithException_ShouldCreateSafeGenericErrorResult()
    {
        // Arrange
        var exception = new InvalidOperationException("Unexpected failure");

        // Act
        var result = ResultsFactory.Error<string>(exception);

        // Assert
        result.Status.ShouldBe(ResultStatus.Error);
        result.Value.ShouldBeNull();
        result.Messages.Count.ShouldBe(1);
        result.Messages[0].Message.ShouldBe("An unexpected error occurred.");
        result.Messages[0].Code.ShouldBeNull();
        result.Messages[0].Severity.ShouldBe(ResultMessageSeverity.Error);
    }

    [Fact]
    public void ErrorWithDiagnosticDetailsOfT_WithException_ShouldPreserveExceptionMessageAndCode()
    {
        var exception = new InvalidOperationException("Unexpected failure");

        var result = ResultsFactory.ErrorWithDiagnosticDetails<string>(exception);

        result.Status.ShouldBe(ResultStatus.Error);
        result.Value.ShouldBeNull();
        result.Messages.Single().Message.ShouldBe(exception.Message);
        result.Messages.Single().Code.ShouldBe(nameof(InvalidOperationException));
    }

    [Fact]
    public void ErrorOfT_WithNullException_ShouldThrowArgumentNullException()
    {
        // Arrange
        Exception exception = null!;

        // Act
        var thrown = Should.Throw<ArgumentNullException>(() => ResultsFactory.Error<string>(exception));

        // Assert
        thrown.ParamName.ShouldBe(nameof(exception));
    }

    [Fact]
    public void ErrorWithDiagnosticDetails_WhenExceptionIsNull_ShouldThrowArgumentNullException()
    {
        Exception exception = null!;

        var thrown = Should.Throw<ArgumentNullException>(() => ResultsFactory.ErrorWithDiagnosticDetails(exception));

        thrown.ParamName.ShouldBe(nameof(exception));
    }

    [Fact]
    public void ErrorWithDiagnosticDetailsOfT_WhenExceptionIsNull_ShouldThrowArgumentNullException()
    {
        Exception exception = null!;

        var thrown = Should.Throw<ArgumentNullException>(() => ResultsFactory.ErrorWithDiagnosticDetails<string>(exception));

        thrown.ParamName.ShouldBe(nameof(exception));
    }
}
