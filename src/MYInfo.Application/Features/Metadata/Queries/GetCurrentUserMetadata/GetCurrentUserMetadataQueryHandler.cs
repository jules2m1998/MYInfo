namespace MYInfo.Application.Features.Metadata.Queries.GetCurrentUserMetadata;

public class GetCurrentUserMetadataQueryHandler(IUserMetadataRepository repository, IUserContextService userContext) : IQueryHandler<GetCurrentUserMetadataQuery, GetCurrentUserMetadataResult>
{
    public async Task<ErrorOr<GetCurrentUserMetadataResult>> Handle(GetCurrentUserMetadataQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserIdentifier();
        if (string.IsNullOrEmpty(userId))
        {
            return Error.Unauthorized("UserMetadata.Unauthorized", "You're not authorized to read this ressource");
        }
        var result = await repository.GetByUserIdAsync(userId);
        if (result == null)
        {
            return Error.NotFound("UserMetadata.NotFound", "This metadata doesn't exists!");
        }
        var dto = new GetCurrentUserMetadataDto(
            result.Id.Value,
            result.Description,
            [.. result.Emails],
            [.. result.PhoneNumbers],
            result.SocialLinks.Adapt<List<GetCurrentUserMetadataDtoSocialLinks>>()
        );

        return new GetCurrentUserMetadataResult(dto);
    }
}
