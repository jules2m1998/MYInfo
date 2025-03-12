namespace MYInfo.API.Endpoints.UserMetadata;
public record CreateMetadataRequest(CreateMetadataDataDto Dto);
public record CreateMetadataResponse(Guid Id);

public class CreateMetadataEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("userMetadata", async (CreateMetadataRequest request, ISender sender) =>
        {
            var command = new CreateMetadataCommand(request.Dto);
            var result = await sender.Send(command);
            var value = result.Value;

            return Results.CreatedAtRoute(nameof(GetUserMetadataByIdEndpoint), new { value.Id }, new CreateMetadataResponse(value.Id));
        })
        .WithName(nameof(CreateMetadataEndpoint))
        .WithTags("User Metadata")
        .WithSummary("Create user metadata")
        .WithDescription("Creates new metadata for a user and returns the unique identifier.")
        .Accepts<CreateMetadataRequest>("application/json")
        .Produces<CreateMetadataResponse>(StatusCodes.Status201Created, "application/json")
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();
    }
}
