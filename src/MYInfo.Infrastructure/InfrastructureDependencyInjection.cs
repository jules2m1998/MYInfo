using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MYInfo.Domain.Repositories;
using MYInfo.Infrastructure.Persistence;
using MYInfo.Infrastructure.Persistence.Data;
using MYInfo.Infrastructure.Persistence.Data.Interceptors;
using MYInfo.Infrastructure.Persistence.Data.Repositories;

namespace MYInfo.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection @this, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("myinfo-data");

        @this.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        @this.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
        @this.AddScoped<IUserMetadataRepository, UserMetadataRepository>();


        @this.AddScoped<IDbContext, MYInfoDbContext>();

        @this.AddDbContext<MYInfoDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString).UseSnakeCaseNamingConvention();
        });
        return @this;
    }


    public static IHost MigrateDatabase<TContext>(this IHost host) where TContext : DbContext
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetService<TContext>();
            context?.Database.Migrate();
        }

        return host;
    }
}
