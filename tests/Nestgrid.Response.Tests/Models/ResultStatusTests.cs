using Shouldly;

namespace Nestgrid.Response.Tests.Models;

public sealed class ResultStatusTests
{
    [Fact]
    public void Values_ShouldMatchApprovedApi()
    {
        // Arrange
        var expected = new[]
        {
            ResultStatus.Ok,
            ResultStatus.Created,
            ResultStatus.Accepted,
            ResultStatus.NoContent,
            ResultStatus.Invalid,
            ResultStatus.NotFound,
            ResultStatus.Unauthorized,
            ResultStatus.Forbidden,
            ResultStatus.Conflict,
            ResultStatus.Cancelled,
            ResultStatus.Failed,
            ResultStatus.Error
        };

        // Act
        var values = Enum.GetValues<ResultStatus>();

        // Assert
        values.ShouldBe(expected);
    }
}
