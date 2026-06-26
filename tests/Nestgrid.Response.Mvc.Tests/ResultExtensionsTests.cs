using System.Text.Json;
using Nestgrid.Response.Mvc.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Text;
using Nestgrid.Response.Http.Options;

namespace Nestgrid.Response.Mvc.Tests;

public sealed class ResultExtensionsTests
{
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
    public async Task ToActionResult_ShouldUseDefaultMapping()
    {
        // Arrange
        var result = ResultsFactory.NotFound("Missing");
        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult().ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task ToActionResult_WithPerCallOptions_ShouldUseCustomMapping()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var options = new NestgridResponseOptions();
        options.StatusMappings[ResultStatus.Failed] = 400;

        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult(options).ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(400);
    }

    [Fact]
    public async Task ToActionResult_WithGenericResultAndPerCallOptions_ShouldUseCustomMapping()
    {
        // Arrange
        var result = ResultsFactory.Failed<UserDto>("Operation failed");
        var options = new NestgridResponseOptions();
        options.StatusMappings[ResultStatus.Failed] = 400;

        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult(options).ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(400);
    }

    [Fact]
    public async Task ToActionResult_WithGlobalOptions_ShouldResolveOptionsAtExecution()
    {
        // Arrange
        var result = ResultsFactory.Cancelled("Stopped");
        var services = new ServiceCollection();
        services.AddNestgridResponse(options =>
            options.StatusMappings[ResultStatus.Cancelled] = 422);

        var httpContext = CreateHttpContext(services);
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult().ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(422);
    }

    [Fact]
    public async Task ToActionResult_WithGlobalOptionsMissingMapping_ShouldUseDefaultMappingWithoutMutatingGlobalOptions()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var services = new ServiceCollection();
        services.AddNestgridResponse(options => options.StatusMappings.Remove(ResultStatus.Failed));

        var httpContext = CreateHttpContext(services);
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult().ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(422);
        GetOptions(services).StatusMappings.ContainsKey(ResultStatus.Failed).ShouldBeFalse();
    }

    [Fact]
    public async Task ToActionResult_WithPerCallOptions_ShouldOverrideGlobalOptions()
    {
        // Arrange
        var result = ResultsFactory.Failed("Operation failed");
        var services = new ServiceCollection();
        services.AddNestgridResponse(options =>
            options.StatusMappings[ResultStatus.Failed] = 418);

        var perCallOptions = new NestgridResponseOptions();
        perCallOptions.StatusMappings[ResultStatus.Failed] = 400;

        var httpContext = CreateHttpContext(services);
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult(perCallOptions).ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(400);
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
        httpContext.Response.StatusCode.ShouldBe(400);

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
        options.StatusMappings[ResultStatus.NoContent] = 200;

        var httpContext = CreateHttpContext();
        var actionContext = CreateActionContext(httpContext);

        // Act
        await result.ToActionResult(options).ExecuteResultAsync(actionContext);

        // Assert
        httpContext.Response.StatusCode.ShouldBe(200);
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
        services.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory>(
            Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        services.AddSingleton<IActionResultExecutor<ObjectResult>, JsonObjectResultExecutor>();

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

    private sealed class JsonObjectResultExecutor : IActionResultExecutor<ObjectResult>
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public Task ExecuteAsync(ActionContext context, ObjectResult result)
        {
            context.HttpContext.Response.StatusCode = result.StatusCode ?? 200;

            var json = JsonSerializer.Serialize(result.Value, JsonOptions);
            var bytes = Encoding.UTF8.GetBytes(json);

            return context.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
