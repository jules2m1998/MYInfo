var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApi(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);
builder.Services.AddCarter();
builder.Services.AddHttpContextAccessor();
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCorsPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()  // Allows requests from any origin
            .AllowAnyMethod()  // Allows all HTTP methods (GET, POST, PUT, DELETE, etc.)
            .AllowAnyHeader(); // Allows all headers
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("DevelopmentCorsPolicy");
}

app.UseCustomConfig();

app.MapGroup("/api/v1").MapCarter();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();

#pragma warning disable S1118 // Utility classes should not have public constructors
public partial class Program;
#pragma warning restore S1118 // Utility classes should not have public constructors