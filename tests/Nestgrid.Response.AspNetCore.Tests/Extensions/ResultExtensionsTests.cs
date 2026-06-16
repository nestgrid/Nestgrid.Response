using System.Text.Json;
using Nestgrid.Response.AspNetCore.Extensions;
using Nestgrid.Response.AspNetCore.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Nestgrid.Response.AspNetCore.Tests.Extensions;

public sealed class ResultExtensionsTests
{
    [Fact]
    public void ToIResult_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result result = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToIResult());

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void ToIResult_WhenGenericResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result<UserDto> result = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToIResult());

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void ToIResult_WhenResultWithOptionsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result result = null!;
        var options = new NestgridResponseOptions();

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToIResult(options));

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void ToIResult_WhenOptionsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok();
        NestgridResponseOptions options = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToIResult(options));

        // Assert
        exception.ParamName.ShouldBe(nameof(options));
    }

    [Fact]
    public void ToIResult_WhenGenericResultWithOptionsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result<UserDto> result = null!;
        var options = new NestgridResponseOptions();

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToIResult(options));

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void ToIResult_WhenGenericOptionsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok(new UserDto(1, "Ada"));
        NestgridResponseOptions options = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToIResult(options));

        // Assert
        exception.ParamName.ShouldBe(nameof(options));
    }

    [Fact]
    public async Task ExecuteAsync_WhenHttpContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok();
        HttpContext httpContext = null!;

        // Act
        var exception = await Should.ThrowAsync<ArgumentNullException>(() =>
            result.ToIResult().ExecuteAsync(httpContext));

        // Assert
        exception.ParamName.ShouldBe(nameof(httpContext));
    }

    [Fact]
    public async Task ToIResult_ShouldUseDefaultMapping()
    {
        // Arrange
        var result = ResultsFactory.Invalid("Name is required");
        var httpContext = CreateHttpContext();

        // Act
        await result.ToIResult().ExecuteAsync(httpContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task ToIResult_WithPerCallOptions_ShouldUseCustomMapping()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var options = new NestgridResponseOptions();
        options.StatusMappings[ResultStatus.Failed] = StatusCodes.Status400BadRequest;

        var httpContext = CreateHttpContext();

        // Act
        await result.ToIResult(options).ExecuteAsync(httpContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task ToIResult_WithGlobalOptions_ShouldResolveOptionsAtExecution()
    {
        // Arrange
        var result = ResultsFactory.Cancelled("Stopped");
        var services = new ServiceCollection();
        services.AddNestgridResponse(options =>
            options.StatusMappings[ResultStatus.Cancelled] = StatusCodes.Status422UnprocessableEntity);

        var httpContext = CreateHttpContext(services);

        // Act
        await result.ToIResult().ExecuteAsync(httpContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public async Task ToIResult_WithGlobalOptionsMissingMapping_ShouldUseDefaultMappingWithoutMutatingGlobalOptions()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var services = new ServiceCollection();
        services.AddNestgridResponse(options => options.StatusMappings.Remove(ResultStatus.Failed));

        var httpContext = CreateHttpContext(services);

        // Act
        await result.ToIResult().ExecuteAsync(httpContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
        GetOptions(services).StatusMappings.ContainsKey(ResultStatus.Failed).ShouldBeFalse();
    }

    [Fact]
    public async Task ToIResult_WithPerCallOptions_ShouldOverrideGlobalOptions()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var globalServices = new ServiceCollection();
        globalServices.AddNestgridResponse(options =>
            options.StatusMappings[ResultStatus.Failed] = StatusCodes.Status418ImATeapot);

        var perCallOptions = new NestgridResponseOptions();
        perCallOptions.StatusMappings[ResultStatus.Failed] = StatusCodes.Status400BadRequest;

        var httpContext = CreateHttpContext(globalServices);

        // Act
        await result.ToIResult(perCallOptions).ExecuteAsync(httpContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task ToIResult_WithPerCallOptions_ShouldNotMutateGlobalOptions()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var services = new ServiceCollection();
        services.AddNestgridResponse(options =>
            options.StatusMappings[ResultStatus.Failed] = StatusCodes.Status418ImATeapot);

        var perCallOptions = new NestgridResponseOptions();
        perCallOptions.StatusMappings[ResultStatus.Failed] = StatusCodes.Status400BadRequest;

        var perCallContext = CreateHttpContext(services);
        var globalContext = CreateHttpContext(services);

        // Act
        await result.ToIResult(perCallOptions).ExecuteAsync(perCallContext);
        await result.ToIResult().ExecuteAsync(globalContext);

        // Assert
        perCallContext.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        globalContext.Response.StatusCode.ShouldBe(StatusCodes.Status418ImATeapot);
    }

    [Fact]
    public async Task ToIResult_WithPerCallOptionsMissingMapping_ShouldUseDefaultMappingWithoutMutatingPerCallOptions()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var options = new NestgridResponseOptions();
        options.StatusMappings.Remove(ResultStatus.Failed);

        var httpContext = CreateHttpContext();

        // Act
        await result.ToIResult(options).ExecuteAsync(httpContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
        options.StatusMappings.ContainsKey(ResultStatus.Failed).ShouldBeFalse();
    }

    [Fact]
    public async Task ToIResult_WithFullResult_ShouldWriteOriginalResult()
    {
        // Arrange
        var result = ResultsFactory.Ok(new UserDto(1, "Ada"));
        var httpContext = CreateHttpContext();

        // Act
        await result.ToIResult().ExecuteAsync(httpContext);

        // Assert
        var json = ReadJson(httpContext);
        json.RootElement.GetProperty("value").GetProperty("id").GetInt32().ShouldBe(1);
        json.RootElement.GetProperty("value").GetProperty("name").GetString().ShouldBe("Ada");
        json.RootElement.GetProperty("messages").GetArrayLength().ShouldBe(0);
        json.RootElement.TryGetProperty("status", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToIResult_WithValueOnly_ShouldWriteValue()
    {
        // Arrange
        var result = ResultsFactory.Ok(new UserDto(1, "Ada"));
        var options = new NestgridResponseOptions { SuccessResponseMode = SuccessResponseMode.ValueOnly };
        var httpContext = CreateHttpContext();

        // Act
        await result.ToIResult(options).ExecuteAsync(httpContext);

        // Assert
        var json = ReadJson(httpContext);
        json.RootElement.GetProperty("id").GetInt32().ShouldBe(1);
        json.RootElement.GetProperty("name").GetString().ShouldBe("Ada");
        json.RootElement.TryGetProperty("value", out _).ShouldBeFalse();
        json.RootElement.TryGetProperty("status", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToIResult_WhenGlobalValueOnly_ShouldWriteGenericValue()
    {
        // Arrange
        var result = ResultsFactory.Ok(new UserDto(1, "Ada"));
        var services = new ServiceCollection();
        services.AddNestgridResponse(options => options.SuccessResponseMode = SuccessResponseMode.ValueOnly);

        var httpContext = CreateHttpContext(services);

        // Act
        await result.ToIResult().ExecuteAsync(httpContext);

        // Assert
        var json = ReadJson(httpContext);
        json.RootElement.GetProperty("id").GetInt32().ShouldBe(1);
        json.RootElement.GetProperty("name").GetString().ShouldBe("Ada");
        json.RootElement.TryGetProperty("value", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToIResult_WhenNonGenericSuccessAndGlobalValueOnly_ShouldWriteOriginalResult()
    {
        // Arrange
        var result = ResultsFactory.Ok();
        var services = new ServiceCollection();
        services.AddNestgridResponse(options => options.SuccessResponseMode = SuccessResponseMode.ValueOnly);

        var httpContext = CreateHttpContext(services);

        // Act
        await result.ToIResult().ExecuteAsync(httpContext);

        // Assert
        var json = ReadJson(httpContext);
        json.RootElement.GetProperty("messages").GetArrayLength().ShouldBe(0);
        json.RootElement.TryGetProperty("value", out _).ShouldBeFalse();
        json.RootElement.TryGetProperty("status", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToIResult_WithValueOnlyFailure_ShouldWriteOriginalResult()
    {
        // Arrange
        var result = ResultsFactory.Invalid<UserDto>("Name is required");
        var options = new NestgridResponseOptions { SuccessResponseMode = SuccessResponseMode.ValueOnly };
        var httpContext = CreateHttpContext();

        // Act
        await result.ToIResult(options).ExecuteAsync(httpContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);

        var json = ReadJson(httpContext);
        json.RootElement.GetProperty("value").ValueKind.ShouldBe(JsonValueKind.Null);
        json.RootElement.GetProperty("messages").GetArrayLength().ShouldBe(1);
        json.RootElement.TryGetProperty("status", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToIResult_WithNoContent_ShouldNeverWriteBody()
    {
        // Arrange
        var result = ResultsFactory.NoContent<UserDto>();
        var options = new NestgridResponseOptions { SuccessResponseMode = SuccessResponseMode.ValueOnly };
        options.StatusMappings[ResultStatus.NoContent] = StatusCodes.Status200OK;

        var httpContext = CreateHttpContext();

        // Act
        await result.ToIResult(options).ExecuteAsync(httpContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
        ReadBody(httpContext).ShouldBeEmpty();
    }

    [Fact]
    public async Task ToActionResult_ShouldUseDefaultMapping()
    {
        // Arrange
        var result = ResultsFactory.NotFound("Missing");
        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult().ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void ToActionResult_WhenResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result result = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToActionResult());

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void ToActionResult_WhenGenericResultIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result<UserDto> result = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToActionResult());

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void ToActionResult_WhenResultWithOptionsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result result = null!;
        var options = new NestgridResponseOptions();

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToActionResult(options));

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void ToActionResult_WhenOptionsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok();
        NestgridResponseOptions options = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToActionResult(options));

        // Assert
        exception.ParamName.ShouldBe(nameof(options));
    }

    [Fact]
    public void ToActionResult_WhenGenericResultWithOptionsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        Result<UserDto> result = null!;
        var options = new NestgridResponseOptions();

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToActionResult(options));

        // Assert
        exception.ParamName.ShouldBe(nameof(result));
    }

    [Fact]
    public void ToActionResult_WhenGenericOptionsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok(new UserDto(1, "Ada"));
        NestgridResponseOptions options = null!;

        // Act
        var exception = Should.Throw<ArgumentNullException>(() => result.ToActionResult(options));

        // Assert
        exception.ParamName.ShouldBe(nameof(options));
    }

    [Fact]
    public async Task ExecuteResultAsync_WhenActionContextIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var result = ResultsFactory.Ok();
        ActionContext actionContext = null!;

        // Act
        var exception = await Should.ThrowAsync<ArgumentNullException>(() =>
            result.ToActionResult().ExecuteResultAsync(actionContext));

        // Assert
        exception.ParamName.ShouldBe("context");
    }

    [Fact]
    public async Task ToActionResult_WithPerCallOptions_ShouldUseCustomMapping()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var options = new NestgridResponseOptions();
        options.StatusMappings[ResultStatus.Failed] = StatusCodes.Status400BadRequest;

        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult(options).ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task ToActionResult_WithGlobalOptions_ShouldResolveOptionsAtExecution()
    {
        // Arrange
        var result = ResultsFactory.Cancelled("Stopped");
        var services = new ServiceCollection();
        services.AddNestgridResponse(options =>
            options.StatusMappings[ResultStatus.Cancelled] = StatusCodes.Status422UnprocessableEntity);

        var httpContext = CreateHttpContext(services);
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult().ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public async Task ToActionResult_WithPerCallOptions_ShouldOverrideGlobalOptions()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var globalServices = new ServiceCollection();
        globalServices.AddNestgridResponse(options =>
            options.StatusMappings[ResultStatus.Failed] = StatusCodes.Status418ImATeapot);

        var perCallOptions = new NestgridResponseOptions();
        perCallOptions.StatusMappings[ResultStatus.Failed] = StatusCodes.Status400BadRequest;

        var httpContext = CreateHttpContext(globalServices);
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult(perCallOptions).ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task ToActionResult_WithPerCallOptions_ShouldNotMutateGlobalOptions()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var services = new ServiceCollection();
        services.AddNestgridResponse(options =>
            options.StatusMappings[ResultStatus.Failed] = StatusCodes.Status418ImATeapot);

        var perCallOptions = new NestgridResponseOptions();
        perCallOptions.StatusMappings[ResultStatus.Failed] = StatusCodes.Status400BadRequest;

        var perCallContext = CreateHttpContext(services);
        var perCallActionContext = CreateActionContext(perCallContext);
        var globalContext = CreateHttpContext(services);
        var globalActionContext = CreateActionContext(globalContext);

        // Act
        await result.ToActionResult(perCallOptions).ExecuteResultAsync(perCallActionContext);
        await result.ToActionResult().ExecuteResultAsync(globalActionContext);

        // Assert
        perCallContext.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        globalContext.Response.StatusCode.ShouldBe(StatusCodes.Status418ImATeapot);
    }

    [Fact]
    public async Task ToActionResult_WithPerCallOptionsMissingMapping_ShouldUseDefaultMappingWithoutMutatingPerCallOptions()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var options = new NestgridResponseOptions();
        options.StatusMappings.Remove(ResultStatus.Failed);

        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult(options).ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status422UnprocessableEntity);
        options.StatusMappings.ContainsKey(ResultStatus.Failed).ShouldBeFalse();
    }

    [Fact]
    public async Task ToActionResult_WithFullResult_ShouldWriteOriginalResult()
    {
        // Arrange
        var result = ResultsFactory.Ok(new UserDto(1, "Ada"));
        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult().ExecuteResultAsync(actionContext);

        // Assert
        var json = ReadJson(httpContext);
        json.RootElement.GetProperty("value").GetProperty("id").GetInt32().ShouldBe(1);
        json.RootElement.GetProperty("value").GetProperty("name").GetString().ShouldBe("Ada");
        json.RootElement.GetProperty("messages").GetArrayLength().ShouldBe(0);
        json.RootElement.TryGetProperty("status", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToActionResult_WithValueOnly_ShouldWriteValue()
    {
        // Arrange
        var result = ResultsFactory.Ok(new UserDto(1, "Ada"));
        var options = new NestgridResponseOptions { SuccessResponseMode = SuccessResponseMode.ValueOnly };
        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult(options).ExecuteResultAsync(actionContext);

        // Assert
        var json = ReadJson(httpContext);
        json.RootElement.GetProperty("id").GetInt32().ShouldBe(1);
        json.RootElement.GetProperty("name").GetString().ShouldBe("Ada");
        json.RootElement.TryGetProperty("value", out _).ShouldBeFalse();
        json.RootElement.TryGetProperty("status", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToActionResult_WhenGlobalValueOnly_ShouldWriteGenericValue()
    {
        // Arrange
        var result = ResultsFactory.Ok(new UserDto(1, "Ada"));
        var services = new ServiceCollection();
        services.AddNestgridResponse(options => options.SuccessResponseMode = SuccessResponseMode.ValueOnly);

        var httpContext = CreateHttpContext(services);
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult().ExecuteResultAsync(actionContext);

        // Assert
        var json = ReadJson(httpContext);
        json.RootElement.GetProperty("id").GetInt32().ShouldBe(1);
        json.RootElement.GetProperty("name").GetString().ShouldBe("Ada");
        json.RootElement.TryGetProperty("value", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToActionResult_WhenNonGenericSuccessAndGlobalValueOnly_ShouldWriteOriginalResult()
    {
        // Arrange
        var result = ResultsFactory.Ok();
        var services = new ServiceCollection();
        services.AddNestgridResponse(options => options.SuccessResponseMode = SuccessResponseMode.ValueOnly);

        var httpContext = CreateHttpContext(services);
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult().ExecuteResultAsync(actionContext);

        // Assert
        var json = ReadJson(httpContext);
        json.RootElement.GetProperty("messages").GetArrayLength().ShouldBe(0);
        json.RootElement.TryGetProperty("value", out _).ShouldBeFalse();
        json.RootElement.TryGetProperty("status", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToActionResult_WithValueOnlyFailure_ShouldWriteOriginalResult()
    {
        // Arrange
        var result = ResultsFactory.Invalid<UserDto>("Name is required");
        var options = new NestgridResponseOptions { SuccessResponseMode = SuccessResponseMode.ValueOnly };
        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult(options).ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);

        var json = ReadJson(httpContext);
        json.RootElement.GetProperty("value").ValueKind.ShouldBe(JsonValueKind.Null);
        json.RootElement.GetProperty("messages").GetArrayLength().ShouldBe(1);
        json.RootElement.TryGetProperty("status", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToActionResult_WithNoContent_ShouldNeverWriteBody()
    {
        // Arrange
        var result = ResultsFactory.NoContent<UserDto>();
        var options = new NestgridResponseOptions { SuccessResponseMode = SuccessResponseMode.ValueOnly };
        options.StatusMappings[ResultStatus.NoContent] = StatusCodes.Status200OK;

        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult(options).ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
        ReadBody(httpContext).ShouldBeEmpty();
    }

    private static ActionContext CreateActionContext(HttpContext httpContext)
    {
        return new ActionContext(
            httpContext,
            new RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
    }

    private static DefaultHttpContext CreateHttpContext(ServiceCollection? services = null)
    {
        services ??= new ServiceCollection();
        services.AddLogging();
        services.AddMvcCore();

        return new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider(),
            Response =
            {
                Body = new MemoryStream()
            }
        };
    }

    private static JsonDocument ReadJson(HttpContext httpContext)
    {
        return JsonDocument.Parse(ReadBody(httpContext));
    }

    private static string ReadBody(HttpContext httpContext)
    {
        httpContext.Response.Body.Position = 0;

        using var reader = new StreamReader(httpContext.Response.Body, leaveOpen: true);

        return reader.ReadToEnd();
    }

    private static NestgridResponseOptions GetOptions(ServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();

        return provider
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<NestgridResponseOptions>>()
            .Value;
    }

    private sealed record UserDto(int Id, string Name);
}
