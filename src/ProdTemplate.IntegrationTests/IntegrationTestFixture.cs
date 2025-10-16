using Microsoft.EntityFrameworkCore;
using ProdTemplate.Api.Services;
using Testcontainers.PostgreSql;

namespace ProdTemplate.IntegrationTests;

public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithCleanUp(true)
        .Build();
    
    public string ConnectionString => _postgresContainer.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        await ApplyMigrations();
    }
    
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _postgresContainer.DisposeAsync();
    }
    
    private async Task ApplyMigrations()
    {
        var options = new DbContextOptionsBuilder<ProdTemplateContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new ProdTemplateContext(options);
        await context.Database.MigrateAsync();
    }
}

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture>
{
}