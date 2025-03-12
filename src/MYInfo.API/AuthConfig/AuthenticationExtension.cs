namespace MYInfo.API.AuthConfig;

public static class AuthenticationExtension
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = configuration["Keycloak:Authority"]!;
            options.Audience = configuration["Keycloak:Audience"];
            options.RequireHttpsMetadata = false;
            options.MetadataAddress = configuration["Keycloak:MetadataAddress"]!;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = configuration["Keycloak:Audience"],
                ValidIssuer = configuration["Keycloak:ValidIssuer"]
            };

            options.Events = new JwtBearerEvents
            {

                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine("Authentication challenge triggered!");
                    Console.WriteLine($"Error: {context.Error}");
                    Console.WriteLine($"Error Description: {context.ErrorDescription}");
                    return Task.CompletedTask;
                }
            };
        });
        services.AddAuthorization();
    }
}
