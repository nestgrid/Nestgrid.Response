using Microsoft.AspNetCore.Mvc;
using Nestgrid.Response.Mvc.Extensions;
using Nestgrid.Response.Mvc.Sample.Models;
using Nestgrid.Response.Mvc.Sample.Services;

namespace Nestgrid.Response.Mvc.Sample.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    private readonly UserService _users;

    public UsersController(UserService users)
    {
        _users = users;
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var result = _users.Get(id);

        return result.ToActionResult();
    }

    [HttpPost]
    public IActionResult Create(UserDto user)
    {
        var result = _users.Create(user);

        return result.ToActionResult();
    }
}
