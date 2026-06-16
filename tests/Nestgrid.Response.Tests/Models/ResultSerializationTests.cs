using System.Text.Json;
using Shouldly;

namespace Nestgrid.Response.Tests.Models;

public sealed class ResultSerializationTests
{
    [Fact]
    public void Serialize_ShouldNotIncludeStatus()
    {
        // Arrange
        var result = ResultsFactory.Invalid("Name is required");

        // Act
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(result));

        // Assert
        document.RootElement.TryGetProperty(nameof(Result.Status), out _).ShouldBeFalse();
    }

    [Fact]
    public void SerializeGenericResult_ShouldNotIncludeStatus()
    {
        // Arrange
        var result = ResultsFactory.Ok(new TestValue(1, "Ada"));

        // Act
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(result));

        // Assert
        document.RootElement.TryGetProperty(nameof(Result.Status), out _).ShouldBeFalse();
    }

    private sealed record TestValue(int Id, string Name);
}
