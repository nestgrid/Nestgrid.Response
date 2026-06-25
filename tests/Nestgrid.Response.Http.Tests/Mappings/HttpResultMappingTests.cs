using Nestgrid.Response.Http.Mappings;
using Shouldly;

namespace Nestgrid.Response.Http.Tests.Mappings;

public sealed class HttpResultMappingTests
{
    [Fact]
    public void ConstructorWithoutBody_ShouldCreateBodylessMapping()
    {
        // Act
        var mapping = new HttpResultMapping(204);

        // Assert
        mapping.StatusCode.ShouldBe(204);
        mapping.HasBody.ShouldBeFalse();
        mapping.Body.ShouldBeNull();
    }

    [Fact]
    public void ConstructorWithBody_ShouldCreateBodyMapping()
    {
        // Arrange
        var body = new { Name = "Ada" };

        // Act
        var mapping = new HttpResultMapping(200, body);

        // Assert
        mapping.StatusCode.ShouldBe(200);
        mapping.HasBody.ShouldBeTrue();
        mapping.Body.ShouldBeSameAs(body);
    }
}
