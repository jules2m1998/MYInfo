
namespace MYInfo.Application.Features.Metadata.Queries.GetUserMetadataById;

public record GetUserMetadataByIdQuery(Guid Id) : IQuery<GetUserMetadataByIdQueryResult>;

public record GetUserMetadataByIdQueryDtoSocialLinks(Uri Url, string Icon, string Title);
public record GetUserMetadataByIdDto(Guid Id, string Description, ICollection<string> Emails, ICollection<string> PhoneNumbers, ICollection<GetUserMetadataByIdQueryDtoSocialLinks> SocialLinks);

public record GetUserMetadataByIdQueryResult(GetUserMetadataByIdDto Value);
