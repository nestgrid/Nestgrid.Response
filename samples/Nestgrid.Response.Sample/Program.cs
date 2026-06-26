using Nestgrid.Response;
using Nestgrid.Response.Extensions;

var user = new User(1, "Ada Lovelace", "ada@example.test");

Print("Ok", Results.Ok(user));
Print("Created", Results.Created(user));
Print("NotFound", Results.NotFound<User>("User was not found."));
Print("Invalid", Results.Invalid<User>("Email is required."));
Print("Error", Results.Error<User>(new InvalidOperationException("The user service failed.")));

var dto = Results.Ok(user)
    .Map(value => new UserDto(value!.Id, value.Name));

Print("Map", dto);

var displayName = dto.Match(
    success => success?.Name ?? "Unknown",
    failure => $"Could not load user: {failure.Status}");

Console.WriteLine();
Console.WriteLine($"Match: {displayName}");

static void Print<T>(string label, Result<T> result)
{
    Console.WriteLine($"{label}: {result.Status}");

    if (result.Value is not null)
    {
        Console.WriteLine($"  Value: {result.Value}");
    }

    foreach (var message in result.Messages)
    {
        Console.WriteLine($"  {message.Severity}: {message.Message}");
    }
}

record User(int Id, string Name, string Email);

record UserDto(int Id, string Name);
