using Nestgrid.Response.Extensions;
using Shouldly;

namespace Nestgrid.Response.Tests.Extensions;

public sealed class ResultMatchExtensionsTests
{
    public static TheoryData<Result<int>, ResultStatus> SuccessfulGenericResults =>
        new()
        {
            { ResultsFactory.Ok(7, ResultMessagesFactory.Info("Loaded")), ResultStatus.Ok },
            { ResultsFactory.Created(7, ResultMessagesFactory.Info("Created")), ResultStatus.Created },
            { ResultsFactory.Accepted(7, ResultMessagesFactory.Info("Accepted")), ResultStatus.Accepted },
            { ResultsFactory.NoContent<int>(), ResultStatus.NoContent }
        };

    public static TheoryData<Result<int>, ResultStatus> NonSuccessfulGenericResults =>
        new()
        {
            { ResultsFactory.Invalid<int>(ResultMessagesFactory.Error("Invalid")), ResultStatus.Invalid },
            { ResultsFactory.NotFound<int>(ResultMessagesFactory.Error("Missing")), ResultStatus.NotFound },
            { ResultsFactory.Unauthorized<int>(ResultMessagesFactory.Error("Unauthenticated")), ResultStatus.Unauthorized },
            { ResultsFactory.Forbidden<int>(ResultMessagesFactory.Error("Forbidden")), ResultStatus.Forbidden },
            { ResultsFactory.Conflict<int>(ResultMessagesFactory.Error("Conflict")), ResultStatus.Conflict },
            { ResultsFactory.Cancelled<int>(ResultMessagesFactory.Error("Cancelled")), ResultStatus.Cancelled },
            { ResultsFactory.Failed<int>(ResultMessagesFactory.Error("Failed")), ResultStatus.Failed },
            { ResultsFactory.Error<int>(ResultMessagesFactory.Error("Error")), ResultStatus.Error }
        };

    public static TheoryData<Result, ResultStatus> SuccessfulResults =>
        new()
        {
            { ResultsFactory.Ok(ResultMessagesFactory.Info("Loaded")), ResultStatus.Ok },
            { ResultsFactory.Created(ResultMessagesFactory.Info("Created")), ResultStatus.Created },
            { ResultsFactory.Accepted(ResultMessagesFactory.Info("Accepted")), ResultStatus.Accepted },
            { ResultsFactory.NoContent(), ResultStatus.NoContent }
        };

    public static TheoryData<Result, ResultStatus> NonSuccessfulResults =>
        new()
        {
            { ResultsFactory.Invalid(ResultMessagesFactory.Error("Invalid")), ResultStatus.Invalid },
            { ResultsFactory.NotFound(ResultMessagesFactory.Error("Missing")), ResultStatus.NotFound },
            { ResultsFactory.Unauthorized(ResultMessagesFactory.Error("Unauthenticated")), ResultStatus.Unauthorized },
            { ResultsFactory.Forbidden(ResultMessagesFactory.Error("Forbidden")), ResultStatus.Forbidden },
            { ResultsFactory.Conflict(ResultMessagesFactory.Error("Conflict")), ResultStatus.Conflict },
            { ResultsFactory.Cancelled(ResultMessagesFactory.Error("Cancelled")), ResultStatus.Cancelled },
            { ResultsFactory.Failed(ResultMessagesFactory.Error("Failed")), ResultStatus.Failed },
            { ResultsFactory.Error(ResultMessagesFactory.Error("Error")), ResultStatus.Error }
        };

    [Theory]
    [MemberData(nameof(SuccessfulGenericResults))]
    public void MatchOfT_WhenResultIsSuccessful_ShouldInvokeSuccess(Result<int> source, ResultStatus expectedStatus)
    {
        // Arrange
        var successInvoked = false;
        var failureInvoked = false;

        // Act
        var result = source.Match(
            value =>
            {
                successInvoked = true;

                return $"Success {value}";
            },
            failure =>
            {
                failureInvoked = true;

                return $"Failure {failure.Status}";
            });

        // Assert
        successInvoked.ShouldBeTrue();
        failureInvoked.ShouldBeFalse();
        result.ShouldBe($"Success {source.Value}");
        source.Status.ShouldBe(expectedStatus);
    }

    [Theory]
    [MemberData(nameof(NonSuccessfulGenericResults))]
    public void MatchOfT_WhenResultIsNotSuccessful_ShouldInvokeFailure(Result<int> source, ResultStatus expectedStatus)
    {
        // Arrange
        var successInvoked = false;
        var failureInvoked = false;
        Result<int>? capturedFailure = null;

        // Act
        var result = source.Match(
            value =>
            {
                successInvoked = true;

                return $"Success {value}";
            },
            failure =>
            {
                failureInvoked = true;
                capturedFailure = failure;

                return $"Failure {failure.Status}";
            });

        // Assert
        successInvoked.ShouldBeFalse();
        failureInvoked.ShouldBeTrue();
        capturedFailure.ShouldBeSameAs(source);
        result.ShouldBe($"Failure {expectedStatus}");
        source.Status.ShouldBe(expectedStatus);
    }

    [Theory]
    [MemberData(nameof(SuccessfulResults))]
    public void Match_WhenResultIsSuccessful_ShouldInvokeSuccess(Result source, ResultStatus expectedStatus)
    {
        // Arrange
        var successInvoked = false;
        var failureInvoked = false;

        // Act
        var result = source.Match(
            () =>
            {
                successInvoked = true;

                return "Success";
            },
            failure =>
            {
                failureInvoked = true;

                return $"Failure {failure.Status}";
            });

        // Assert
        successInvoked.ShouldBeTrue();
        failureInvoked.ShouldBeFalse();
        result.ShouldBe("Success");
        source.Status.ShouldBe(expectedStatus);
    }

    [Theory]
    [MemberData(nameof(NonSuccessfulResults))]
    public void Match_WhenResultIsNotSuccessful_ShouldInvokeFailure(Result source, ResultStatus expectedStatus)
    {
        // Arrange
        var successInvoked = false;
        var failureInvoked = false;
        Result? capturedFailure = null;

        // Act
        var result = source.Match(
            () =>
            {
                successInvoked = true;

                return "Success";
            },
            failure =>
            {
                failureInvoked = true;
                capturedFailure = failure;

                return $"Failure {failure.Status}";
            });

        // Assert
        successInvoked.ShouldBeFalse();
        failureInvoked.ShouldBeTrue();
        capturedFailure.ShouldBeSameAs(source);
        result.ShouldBe($"Failure {expectedStatus}");
        source.Status.ShouldBe(expectedStatus);
    }

    [Fact]
    public void MatchOfT_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result<int> result = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
            result.Match(value => value.ToString(), failure => failure.Status.ToString()));

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
        exception.StackTrace.ShouldStartWith("   at Nestgrid.Response.Extensions.ResultMatchExtensions.Match");
    }

    [Fact]
    public void MatchOfT_WhenSuccessIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok(7);
        Func<int, string> success = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
            result.Match<int, string>(success, failure => failure.Status.ToString()));

        // Assert
        exception.ParamName.ShouldBe(nameof(success));
    }

    [Fact]
    public void MatchOfT_WhenFailureIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok(7);
        Func<Result<int>, string> failure = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
            result.Match(value => value.ToString(), failure));

        // Assert
        exception.ParamName.ShouldBe(nameof(failure));
    }

    [Fact]
    public void Match_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result result = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
            result.Match(() => "Success", failure => failure.Status.ToString()));

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
        exception.StackTrace.ShouldStartWith("   at Nestgrid.Response.Extensions.ResultMatchExtensions.Match");
    }

    [Fact]
    public void Match_WhenSuccessIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok();
        Func<string> success = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
            result.Match(success, failure => failure.Status.ToString()));

        // Assert
        exception.ParamName.ShouldBe(nameof(success));
    }

    [Fact]
    public void Match_WhenFailureIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok();
        Func<Result, string> failure = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
            result.Match(() => "Success", failure));

        // Assert
        exception.ParamName.ShouldBe(nameof(failure));
    }
}
