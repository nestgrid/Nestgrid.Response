using Nestgrid.Response.AspNetCore.Extensions;
using Nestgrid.Response.AspNetCore.Sample.Models;
using Nestgrid.Response.AspNetCore.Sample.Services;
using Nestgrid.Response.Http.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNestgridResponse();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

app.MapGet("/users/{id:int}", (int id, UserService users) =>
{
    var result = users.Get(id);

    return result.ToIResult();
});

app.MapGet("/users/{id:int}/value-only", (int id, UserService users) =>
{
    var result = users.Get(id);
    var options = new NestgridResponseOptions
    {
        SuccessResponseMode = SuccessResponseMode.ValueOnly
    };

    return result.ToIResult(options);
});

app.MapPost("/users", (UserDto user, UserService users) =>
{
    var result = users.Create(user);

    return result.ToIResult();
});

app.MapDelete("/users/{id:int}", (int id, UserService users) =>
{
    var result = users.Delete(id);

    return result.ToIResult();
});

app.Run();
