using Nestgrid.Response.Mvc.Extensions;
using Nestgrid.Response.Mvc.Sample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNestgridResponse();
builder.Services.AddSingleton<UserService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
