using System.Text.Json.Serialization;
using Npgsql;
using ProdTemplate.Api;
using ProdTemplate.Api.Models.Dto.Requests;
using ProdTemplate.Api.Services;
using Serilog;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var services = builder.Services;

services
    .AddPostgres(
        new NpgsqlConnectionStringBuilder
        {
            Host = "postgres",
            Port = 5432,
            Database = builder.Configuration["POSTGRES_DB"],
            Username = builder.Configuration["POSTGRES_USER"],
            Password = builder.Configuration["POSTGRES_PASSWORD"]
        }.ConnectionString)
    //.AddOTel()
    //.AddLogger(builder.Configuration)
    .AddOpenApi()
    .AddExceptionFilter();

services.AddSingleton<IJwtService, JwtService>();
services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.MapPrometheusScrapingEndpoint();
// app.UseSerilogRequestLogging();

app.MapControllers();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ProdTemplateContext>();
context.Database.EnsureCreated();

await app.RunAsync();

namespace ProdTemplate.Api
{
    [JsonSerializable(typeof(SignInRequest))]
    [JsonSerializable(typeof(SignUpRequest))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext;
}