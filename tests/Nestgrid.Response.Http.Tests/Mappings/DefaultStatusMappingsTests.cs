using Nestgrid.Response.Http.Mappings;
using Shouldly;

namespace Nestgrid.Response.Http.Tests.Mappings;

public sealed class DefaultStatusMappingsTests
{
    [Fact]
    public void Create_ShouldReturnDefaultMappings()
    {
        // Act
        var mappings = DefaultStatusMappings.Create();

        // Assert
        mappings[ResultStatus.Ok].ShouldBe(200);
        mappings[ResultStatus.Created].ShouldBe(201);
        mappings[ResultStatus.Accepted].ShouldBe(202);
        mappings[ResultStatus.NoContent].ShouldBe(204);
        mappings[ResultStatus.Invalid].ShouldBe(400);
        mappings[ResultStatus.Unauthorized].ShouldBe(401);
        mappings[ResultStatus.Forbidden].ShouldBe(403);
        mappings[ResultStatus.NotFound].ShouldBe(404);
        mappings[ResultStatus.Conflict].ShouldBe(409);
        mappings[ResultStatus.Cancelled].ShouldBe(409);
        mappings[ResultStatus.Failed].ShouldBe(422);
        mappings[ResultStatus.Error].ShouldBe(500);
        mappings.Count.ShouldBe(Enum.GetValues<ResultStatus>().Length);
    }
}
