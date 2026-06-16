using Nestgrid.Response.AspNetCore.Mappings;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace Nestgrid.Response.AspNetCore.Tests.Mappings;

public sealed class DefaultStatusMappingsTests
{
    [Fact]
    public void Create_ShouldReturnDefaultMappings()
    {
        // Act
        var mappings = DefaultStatusMappings.Create();

        // Assert
        mappings[ResultStatus.Ok].ShouldBe(StatusCodes.Status200OK);
        mappings[ResultStatus.Created].ShouldBe(StatusCodes.Status201Created);
        mappings[ResultStatus.Accepted].ShouldBe(StatusCodes.Status202Accepted);
        mappings[ResultStatus.NoContent].ShouldBe(StatusCodes.Status204NoContent);
        mappings[ResultStatus.Invalid].ShouldBe(StatusCodes.Status400BadRequest);
        mappings[ResultStatus.Unauthorized].ShouldBe(StatusCodes.Status401Unauthorized);
        mappings[ResultStatus.Forbidden].ShouldBe(StatusCodes.Status403Forbidden);
        mappings[ResultStatus.NotFound].ShouldBe(StatusCodes.Status404NotFound);
        mappings[ResultStatus.Conflict].ShouldBe(StatusCodes.Status409Conflict);
        mappings[ResultStatus.Cancelled].ShouldBe(StatusCodes.Status409Conflict);
        mappings[ResultStatus.Failed].ShouldBe(StatusCodes.Status422UnprocessableEntity);
        mappings[ResultStatus.Error].ShouldBe(StatusCodes.Status500InternalServerError);
        mappings.Count.ShouldBe(Enum.GetValues<ResultStatus>().Length);
    }
}
