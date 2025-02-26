using MYInfo.API.Services;
using MYInfo.Application;
using MYInfo.Domain.Services;
using MYInfo.Infrastructure;
using MYInfo.Infrastructure.Persistence.Data;

namespace MYInfo.API;

public static class AddApiDependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection @this, IConfiguration configuration)
    {
        @this.AddScoped<IUserContextService, UserContextService>();
        @this.AddInfrastructure(configuration)
            .AddApplication();
        return @this;
    }

    public static WebApplication UseCustomConfig(this WebApplication @this)
    {
        @this.UseApiDocumentation()
            .MigrateDatabase<MYInfoDbContext>();
        return @this;
    }

    private static WebApplication UseApiDocumentation(this WebApplication @this)
    {
        if (@this.Environment.IsDevelopment())
        {
            @this.UseReDoc(cfg =>
            {
                cfg.DocumentTitle = "MYInfo API Documentation";
                cfg.SpecUrl = "/openapi/v1.json";
            });
        }
        return @this;
    }
}
