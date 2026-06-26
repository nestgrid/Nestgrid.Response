using Nestgrid.Response.AspNetCore.Sample.Models;
using ResponseResults = Nestgrid.Response.Results;

namespace Nestgrid.Response.AspNetCore.Sample.Services;

public sealed class UserService
{
    private readonly Dictionary<int, UserDto> _users = new()
    {
        [1] = new UserDto(1, "Ada Lovelace", "ada@example.test"),
        [2] = new UserDto(2, "Grace Hopper", "grace@example.test")
    };

    public Result<UserDto> Get(int id)
    {
        return _users.TryGetValue(id, out var user)
            ? ResponseResults.Ok(user)
            : ResponseResults.NotFound<UserDto>("User was not found.");
    }

    public Result<UserDto> Create(UserDto user)
    {
        if (string.IsNullOrWhiteSpace(user.Name))
        {
            return ResponseResults.Invalid<UserDto>("Name is required.");
        }

        _users[user.Id] = user;

        return ResponseResults.Created(user);
    }

    public Result<UserDto> Delete(int id)
    {
        if (!_users.Remove(id))
        {
            return ResponseResults.NotFound<UserDto>("User was not found.");
        }

        return ResponseResults.NoContent<UserDto>();
    }
}
