using MYInfo.Application.Features.Metadata.Queries.GetCurrentUserMetadata;

namespace MYInfo.ApiFunctionnalTests.TestData;

public class GetUserMetadataByIdTestData : TheoryData<GetCurrentUserMetadataDto>
{
    public GetUserMetadataByIdTestData()
    {
        Add(new GetCurrentUserMetadataDto(
            Guid.CreateVersion7(),
            "Valid description",
            ["user@example.com"],
            ["+1234567890"],
            [
                new GetCurrentUserMetadataDtoSocialLinks(new Uri("https://linkedin.com/in/user"), "linkedin-icon", "LinkedIn Profile")
            ]
        ));

        Add(new GetCurrentUserMetadataDto(
            Guid.CreateVersion7(),
            "Another valid description",
            ["test.user@domain.com", "hello@company.org"],
            ["+1987654321", "+447123456789"],
            [
                new GetCurrentUserMetadataDtoSocialLinks(new Uri("https://twitter.com/user"), "twitter-icon", "Twitter Page"),
                new GetCurrentUserMetadataDtoSocialLinks(new Uri("https://github.com/user"), "github-icon", "GitHub Profile")
            ]
        ));

        Add(new GetCurrentUserMetadataDto(
            Guid.CreateVersion7(),
            "Social media influencer contact",
            ["contact@influencer.io"],
            ["+15551234567"],
            [
                new GetCurrentUserMetadataDtoSocialLinks(new Uri("https://instagram.com/influencer"), "instagram-icon", "Instagram Page"),
                new GetCurrentUserMetadataDtoSocialLinks(new Uri("https://tiktok.com/@influencer"), "tiktok-icon", "TikTok Profile"),
                new GetCurrentUserMetadataDtoSocialLinks(new Uri("https://youtube.com/influencer"), "youtube-icon", "YouTube Channel")
            ]
        ));
    }
}
