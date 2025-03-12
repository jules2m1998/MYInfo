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
