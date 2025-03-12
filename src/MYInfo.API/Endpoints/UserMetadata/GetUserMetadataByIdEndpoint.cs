namespace MYInfo.API.Endpoints.UserMetadata;

/// <summary>
/// Response model for the Get User Metadata by ID endpoint
/// </summary>
/// <param name="Id">Unique identifier of the user metadata</param>
/// <param name="Description">User's description or bio</param>
/// <param name="Emails">Collection of user's email addresses</param>
/// <param name="PhoneNumbers">Collection of user's phone numbers</param>
/// <param name="SocialLinks">Collection of user's social media links</param>
public record GetUserMetadataByIdResponse(Guid Id, string Description, ICollection<string> Emails, ICollection<string> PhoneNumbers, ICollection<GetUserMetadataByIdQueryDtoSocialLinks> SocialLinks);

/// <summary>
/// Endpoint for retrieving user metadata by its unique identifier
/// </summary>
public class GetUserMetadataByIdEndpoint : ICarterModule
{
    /// <summary>
    /// Configures the routes for the User Metadata API
    /// </summary>
    /// <param name="app">The endpoint route builder</param>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("userMetadata/{id}", async (Guid id, ISender sender, CancellationToken cancellation) =>
        {
            var query = new GetUserMetadataByIdQuery(id);
            var result = await sender.Send(query, cancellation);
            if (result.IsError)
            {
                var problem = result.ToProblemDetails();
                return Results.Problem(
                    detail: problem?.Detail,
                    statusCode: problem?.Status,
                    title: problem?.Title
                );
            }
            var value = result.Value.Value;

            return Results.Ok(value.Adapt<GetUserMetadataByIdResponse>());
        })
        .WithName(nameof(GetUserMetadataByIdEndpoint))
        .WithDescription("Retrieves detailed user metadata by its unique identifier")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get User Metadata by ID";
            operation.Description = "Retrieves a specific user metadata record using its unique identifier";

            operation.Responses["200"].Description = "User metadata retrieved successfully";
            operation.Responses["404"].Description = "User metadata not found";
            operation.Responses["401"].Description = "You're not authorized to read this ressource";
            operation.Responses["500"].Description = "Internal server error occurred";

            return operation;
        })
        .Produces<GetUserMetadataByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();
    }
}