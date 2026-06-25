using Nestgrid.Response.Http.Mappings;
using Nestgrid.Response.Http.Options;
using Shouldly;

namespace Nestgrid.Response.Http.Tests.Mappings;

public sealed class HttpResultMapperTests
{
    [Fact]
    public void Map_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result result = null!;
        var options = new NestgridResponseOptions();

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
            HttpResultMapper.Map(result, options, hasValue: false, value: null));

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void Map_WhenOptionsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok();
        NestgridResponseOptions options = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() =>
            HttpResultMapper.Map(result, options, hasValue: false, value: null));

        // Assert
        exception.ParamName.ShouldBe(nameof(options));
    }

    [Fact]
    public void Map_ShouldUseDefaultStatusMapping()
    {
        // Arrange
        var result = ResultsFactory.Invalid("Name is required");
        var options = new NestgridResponseOptions();

        // Act
        var mapping = HttpResultMapper.Map(result, options, hasValue: false, value: null);

        // Assert
        mapping.StatusCode.ShouldBe(400);
        mapping.HasBody.ShouldBeTrue();
        mapping.Body.ShouldBeSameAs(result);
    }

    [Fact]
    public void Map_WithCustomStatusMapping_ShouldUseCustomMapping()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var options = new NestgridResponseOptions();
        options.StatusMappings[ResultStatus.Failed] = 400;

        // Act
        var mapping = HttpResultMapper.Map(result, options, hasValue: false, value: null);

        // Assert
        mapping.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Map_WhenMappingIsMissing_ShouldFallBackToDefaultMapping()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var options = new NestgridResponseOptions();
        options.StatusMappings.Remove(ResultStatus.Failed);

        // Act
        var mapping = HttpResultMapper.Map(result, options, hasValue: false, value: null);

        // Assert
        mapping.StatusCode.ShouldBe(422);
        options.StatusMappings.ContainsKey(ResultStatus.Failed).ShouldBeFalse();
    }

    [Fact]
    public void Map_WhenResultIsNoContent_ShouldSuppressBody()
    {
        // Arrange
        var result = ResultsFactory.NoContent<UserDto>();
        var options = new NestgridResponseOptions { SuccessResponseMode = SuccessResponseMode.ValueOnly };
        options.StatusMappings[ResultStatus.NoContent] = 200;

        // Act
        var mapping = HttpResultMapper.Map(result, options, hasValue: true, value: result.Value);

        // Assert
        mapping.StatusCode.ShouldBe(200);
        mapping.HasBody.ShouldBeFalse();
        mapping.Body.ShouldBeNull();
    }

    [Fact]
    public void Map_WithFullResult_ShouldReturnResultBody()
    {
        // Arrange
        var result = ResultsFactory.Ok(new UserDto(1, "Ada"));
        var options = new NestgridResponseOptions();

        // Act
        var mapping = HttpResultMapper.Map(result, options, hasValue: true, value: result.Value);

        // Assert
        mapping.StatusCode.ShouldBe(200);
        mapping.HasBody.ShouldBeTrue();
        mapping.Body.ShouldBeSameAs(result);
    }

    [Fact]
    public void Map_WithValueOnlySuccessfulGenericResult_ShouldReturnValueBody()
    {
        // Arrange
        var result = ResultsFactory.Ok(new UserDto(1, "Ada"));
        var options = new NestgridResponseOptions { SuccessResponseMode = SuccessResponseMode.ValueOnly };

        // Act
        var mapping = HttpResultMapper.Map(result, options, hasValue: true, value: result.Value);

        // Assert
        mapping.StatusCode.ShouldBe(200);
        mapping.HasBody.ShouldBeTrue();
        mapping.Body.ShouldBe(result.Value);
    }

    [Fact]
    public void Map_WithValueOnlyNonGenericResult_ShouldReturnResultBody()
    {
        // Arrange
        var result = ResultsFactory.Ok();
        var options = new NestgridResponseOptions { SuccessResponseMode = SuccessResponseMode.ValueOnly };

        // Act
        var mapping = HttpResultMapper.Map(result, options, hasValue: false, value: null);

        // Assert
        mapping.HasBody.ShouldBeTrue();
        mapping.Body.ShouldBeSameAs(result);
    }

    [Fact]
    public void Map_WithValueOnlyFailure_ShouldReturnResultBody()
    {
        // Arrange
        var result = ResultsFactory.Invalid<UserDto>("Name is required");
        var options = new NestgridResponseOptions { SuccessResponseMode = SuccessResponseMode.ValueOnly };

        // Act
        var mapping = HttpResultMapper.Map(result, options, hasValue: true, value: result.Value);

        // Assert
        mapping.StatusCode.ShouldBe(400);
        mapping.HasBody.ShouldBeTrue();
        mapping.Body.ShouldBeSameAs(result);
    }

    private sealed record UserDto(int Id, string Name);
}
