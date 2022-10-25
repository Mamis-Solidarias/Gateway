using MamisSolidarias.Gateway.Extensions;
using MamisSolidarias.HttpClient.Users;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection(builder.Configuration);
builder.Services.AddAuthEndpoints(builder.Configuration,builder.Environment);
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddYarp(builder.Configuration);
builder.Services.AddOpenTelemetry(builder.Configuration, builder.Environment);
builder.Services.AddGraphQl(builder.Configuration);
builder.AddUsersHttpClient();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuth();
app.UseAuthEndpoints();
app.UseGraphQl();
app.UseYarp();


app.Run();