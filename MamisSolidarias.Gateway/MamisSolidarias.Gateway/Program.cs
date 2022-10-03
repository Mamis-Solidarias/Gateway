using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using MamisSolidarias.Gateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddYamlFile("appsettings.yml", optional: true, reloadOnChange: true);

builder.Services.AddAuthEndpoints(builder.Configuration,builder.Environment);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthEndpoints();

app.Run();