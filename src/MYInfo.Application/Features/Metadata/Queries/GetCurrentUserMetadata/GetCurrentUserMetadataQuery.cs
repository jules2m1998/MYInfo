namespace MYInfo.Application.Features.Metadata.Queries.GetCurrentUserMetadata;

public class GetCurrentUserMetadataQuery() : IQuery<GetCurrentUserMetadataResult>;

public record GetCurrentUserMetadataDtoSocialLinks(Uri Url, string Icon, string Title);
public record GetCurrentUserMetadataDto(Guid Id, string Description, ICollection<string> Emails, ICollection<string> PhoneNumbers, ICollection<GetCurrentUserMetadataDtoSocialLinks> SocialLinks);

public record GetCurrentUserMetadataResult(GetCurrentUserMetadataDto Value);