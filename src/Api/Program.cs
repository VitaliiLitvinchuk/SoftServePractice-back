using Api.Dependencies;
using Api.Modules.Database;
using Api.Modules.RouteFiltering;
using Api.Modules.Validator;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Application;
using Api.Modules.Auth;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
    options.Filters.Add<ValidationExceptionFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy => policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "http://localhost:3000", "https://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddValidators();
builder.Services.UseAuthenticationScheme(builder.Configuration);
builder.Services.UseSwaggerGen();

var app = builder.Build();

app.UseSwagger(options =>
{
    options.RouteTemplate = "/openapi/{documentName}.json";
});

app.MapScalarApiReference("s", options =>
{
});

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.ConfigureStaticFiles();

app.UseHangfireDashboardAndJobs();

app
    .UseAuthentication()
    .UseAuthorization();

app.UseCors("AllowLocalhost");

await app.InitializeDb();

app.MapControllers();

app.UseHttpsRedirection();

await app.RunAsync();

public partial class Program;