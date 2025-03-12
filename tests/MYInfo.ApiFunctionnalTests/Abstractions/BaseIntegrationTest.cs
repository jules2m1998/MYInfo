using EFCore.BulkExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MYInfo.ApiFunctionnalTests.Tools;
using MYInfo.Infrastructure.Persistence.Data;
using System.Net.Http.Headers;

namespace MYInfo.ApiFunctionnalTests.Abstractions;


public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>,
      IDisposable
{
    private readonly IServiceScope _scope;
    private readonly MYInfoDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private bool _disposed;

    protected MYInfoDbContext DbContext => _dbContext;
    protected HttpClient Client => _httpClient;
    protected IConfiguration Configuration => _configuration;


    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        _dbContext = _scope.ServiceProvider
            .GetRequiredService<MYInfoDbContext>();
        _httpClient = factory.CreateClient();
        _configuration = _scope.ServiceProvider.GetRequiredService<IConfiguration>();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _scope.Dispose();
                _dbContext.Dispose();
                _httpClient.Dispose();
            }

            // Free unmanaged resources here (if any)

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void ApplyAutorisation()
    {
        var validToken = JwtTokenGenerator.GenerateValidToken(_configuration);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", validToken);
    }

    protected async Task ClearTableAsync<TEntity>()
        where TEntity : class
    {
        await _dbContext.BulkDeleteAsync(_dbContext.Set<TEntity>());
    }
}
