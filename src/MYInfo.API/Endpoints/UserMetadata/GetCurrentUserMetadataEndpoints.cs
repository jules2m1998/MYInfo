namespace MYInfo.API.Endpoints.UserMetadata;

public record GetCurrentUserMetadataResponse(Guid Id, string Description, ICollection<string> Emails, ICollection<string> PhoneNumbers, ICollection<GetCurrentUserMetadataDtoSocialLinks> SocialLinks);

public class GetCurrentUserMetadataEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("userMetadata/current", async (ISender sender, CancellationToken cancellation) =>
        {
            var query = new GetCurrentUserMetadataQuery();
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
            return Results.Ok(value.Adapt<GetCurrentUserMetadataResponse>());
        })
        .WithName(nameof(GetCurrentUserMetadataEndpoints))
        .WithDescription("Retrieves metadata for the currently authenticated user")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get Current User Metadata";
            operation.Description = "Retrieves the metadata associated with the currently authenticated user";

            // Add response documentation
            operation.Responses["200"].Description = "User metadata retrieved successfully";
            operation.Responses["401"].Description = "User is not authenticated";
            operation.Responses["404"].Description = "User metadata not found";
            operation.Responses["500"].Description = "Internal server error occurred";

            return operation;
        })
        .Produces<GetCurrentUserMetadataResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();
    }
}
