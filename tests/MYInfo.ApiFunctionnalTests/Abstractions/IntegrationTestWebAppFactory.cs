using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Testcontainers.MsSql;

namespace MYInfo.ApiFunctionnalTests.Abstractions;

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:myinfo-data", _dbContainer.GetConnectionString());

        // Authentication
        Environment.SetEnvironmentVariable("JwtTest:Secret", ";+cna>!nyeIw/Bm2L5-/LdI]n}Nx;wqadyW0gU%UE{>XjLZJa;bG_L0h~]|}'q0");
        Environment.SetEnvironmentVariable("JwtTest:Audience", "api-test");
        Environment.SetEnvironmentVariable("JwtTest:UserName", "UserName");
        Environment.SetEnvironmentVariable("JwtTest:UserId", Guid.CreateVersion7().ToString());
        Environment.SetEnvironmentVariable("JwtTest:Issuer", "https://keycloak-test");

        builder.ConfigureTestServices(services =>
        {
            using var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            // Override the JwtBearer options for testing.
            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                // Use test values (simulate Keycloak's config)
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtTest:Issuer"], // Test issuer
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtTest:Audience"],            // Test audience
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtTest:Secret"]!)) // Test key
                };

                // Optionally disable retrieving metadata from Keycloak in tests:
                options.RequireHttpsMetadata = false;
                options.Authority = null;
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
