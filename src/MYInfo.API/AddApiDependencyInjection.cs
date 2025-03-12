namespace MYInfo.API;

public static class AddApiDependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection @this, IConfiguration configuration)
    {

        @this.AddScoped<IUserContextService, UserContextService>();
        @this.AddInfrastructure(configuration)
            .AddApplication();
        @this.AddExceptionHandler<CustomExceptionHandler>();
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
            @this.MapScalarApiReference(); // scalar/v1
        }
        @this.UseExceptionHandler(opt => { });
        return @this;
    }
}
