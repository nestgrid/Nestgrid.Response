using Nestgrid.Response.Extensions;
using Shouldly;

namespace Nestgrid.Response.Tests.Extensions;

public sealed class ResultExtensionsTests
{
    public static TheoryData<Result, ResultStatus> SuccessfulResults =>
        new()
        {
            { ResultsFactory.Ok(), ResultStatus.Ok },
            { ResultsFactory.Created(), ResultStatus.Created },
            { ResultsFactory.Accepted(), ResultStatus.Accepted },
            { ResultsFactory.NoContent(), ResultStatus.NoContent }
        };

    public static TheoryData<Result, ResultStatus> FailureResults =>
        new()
        {
            { ResultsFactory.Invalid(), ResultStatus.Invalid },
            { ResultsFactory.Unauthorized(), ResultStatus.Unauthorized },
            { ResultsFactory.Forbidden(), ResultStatus.Forbidden },
            { ResultsFactory.NotFound(), ResultStatus.NotFound },
            { ResultsFactory.Conflict(), ResultStatus.Conflict },
            { ResultsFactory.Cancelled(), ResultStatus.Cancelled },
            { ResultsFactory.Failed(), ResultStatus.Failed },
            { ResultsFactory.Error(), ResultStatus.Error }
        };

    [Theory]
    [MemberData(nameof(SuccessfulResults))]
    public void IsSuccess_WhenResultHasSuccessStatus_ShouldReturnTrue(Result result, ResultStatus expectedStatus)
    {
        // Arrange

        // Act
        var isSuccess = result.IsSuccess();

        // Assert
        result.Status.ShouldBe(expectedStatus);
        isSuccess.ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(FailureResults))]
    public void IsSuccess_WhenResultHasFailureStatus_ShouldReturnFalse(Result result, ResultStatus expectedStatus)
    {
        // Arrange

        // Act
        var isSuccess = result.IsSuccess();

        // Assert
        result.Status.ShouldBe(expectedStatus);
        isSuccess.ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(SuccessfulResults))]
    public void IsFailure_WhenResultHasSuccessStatus_ShouldReturnFalse(Result result, ResultStatus expectedStatus)
    {
        // Arrange

        // Act
        var isFailure = result.IsFailure();

        // Assert
        result.Status.ShouldBe(expectedStatus);
        isFailure.ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(FailureResults))]
    public void IsFailure_WhenResultHasFailureStatus_ShouldReturnTrue(Result result, ResultStatus expectedStatus)
    {
        // Arrange

        // Act
        var isFailure = result.IsFailure();

        // Assert
        result.Status.ShouldBe(expectedStatus);
        isFailure.ShouldBeTrue();
    }

    [Fact]
    public void IsSuccess_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result result = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.IsSuccess());

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void IsFailure_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result result = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.IsFailure());

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }
}
