

namespace MYInfo.Application.Features.Metadata.Queries.GetUserMetadataById;

public class GetUserMetadataByIdHandler(IUserMetadataRepository repository) : IQueryHandler<GetUserMetadataByIdQuery, GetUserMetadataByIdQueryResult>
{

    public async Task<ErrorOr<GetUserMetadataByIdQueryResult>> Handle(GetUserMetadataByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await repository.GetByIdAsync(UserMetaDataId.Of(request.Id), cancellationToken);
        if (data == null)
        {
            return Error.NotFound("UserMetadata.NotFound", "This metadata doesn't exists!");
        }
        var dto = new GetUserMetadataByIdDto(
            data.Id.Value,
            data.Description,
            [.. data.Emails],
            [.. data.PhoneNumbers],
            data.SocialLinks.Adapt<List<GetUserMetadataByIdQueryDtoSocialLinks>>());

        return new GetUserMetadataByIdQueryResult(dto);
    }
}
