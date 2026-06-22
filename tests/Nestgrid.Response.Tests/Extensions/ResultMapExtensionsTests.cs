using Nestgrid.Response.Extensions;
using Shouldly;

namespace Nestgrid.Response.Tests.Extensions;

public sealed class ResultMapExtensionsTests
{
    public static TheoryData<Result<int>, ResultStatus> SuccessfulResults =>
        new()
        {
            { ResultsFactory.Ok(7, ResultMessagesFactory.Info("Loaded")), ResultStatus.Ok },
            { ResultsFactory.Created(7, ResultMessagesFactory.Info("Created")), ResultStatus.Created },
            { ResultsFactory.Accepted(7, ResultMessagesFactory.Info("Accepted")), ResultStatus.Accepted },
            { ResultsFactory.NoContent<int>(), ResultStatus.NoContent }
        };

    public static TheoryData<Result<int>, ResultStatus> NonSuccessfulResults =>
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

    [Theory]
    [MemberData(nameof(SuccessfulResults))]
    public void Map_WhenResultIsSuccessful_ShouldInvokeMapper(Result<int> source, ResultStatus expectedStatus)
    {
        // Arrange
        var invoked = false;

        // Act
        var result = source.Map(value =>
        {
            invoked = true;

            return $"Mapped {value}";
        });

        // Assert
        invoked.ShouldBeTrue();
        result.Status.ShouldBe(expectedStatus);
        result.Value.ShouldBe($"Mapped {source.Value}");
        result.Messages.ShouldBe(source.Messages);
    }

    [Theory]
    [MemberData(nameof(NonSuccessfulResults))]
    public void Map_WhenResultIsNotSuccessful_ShouldNotInvokeMapper(Result<int> source, ResultStatus expectedStatus)
    {
        // Arrange
        var invoked = false;

        // Act
        var result = source.Map(value =>
        {
            invoked = true;

            return $"Mapped {value}";
        });

        // Assert
        invoked.ShouldBeFalse();
        result.Status.ShouldBe(expectedStatus);
        result.Value.ShouldBeNull();
        result.Messages.ShouldBe(source.Messages);
    }

    [Fact]
    public void Map_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result<int> result = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.Map(value => value.ToString()));

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void Map_WhenMapperIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok(7);
        Func<int, string> mapper = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.Map<int, string>(mapper));

        // Assert
        exception.ParamName.ShouldBe(nameof(mapper));
    }
}
