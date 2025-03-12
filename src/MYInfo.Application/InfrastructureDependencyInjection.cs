namespace MYInfo.Application;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection @this)
    {

        @this.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        @this.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        @this.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        @this.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return @this;
    }
}
