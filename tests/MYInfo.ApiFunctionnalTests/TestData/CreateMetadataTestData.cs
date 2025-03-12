using MYInfo.Application.Features.Metadata.Commands.CreateMetadata;

namespace MYInfo.ApiFunctionnalTests.TestData;

public class CreateMetadataTestData : TheoryData<CreateMetadataDataDto>
{
    public CreateMetadataTestData()
    {
        Add(new CreateMetadataDataDto(
            "Valid description",
            ["user@example.com"],
            ["+1234567890"],
            [
                new CreateMetadataDataDtoSocialLinks(new Uri("https://linkedin.com/in/user"), "linkedin-icon", "LinkedIn Profile")
            ]
        ));

        Add(new CreateMetadataDataDto(
            "Another valid description",
            ["test.user@domain.com", "hello@company.org"],
            ["+1987654321", "+447123456789"],
            [
                new CreateMetadataDataDtoSocialLinks(new Uri("https://twitter.com/user"), "twitter-icon", "Twitter Page"),
                new CreateMetadataDataDtoSocialLinks(new Uri("https://github.com/user"), "github-icon", "GitHub Profile")
            ]
        ));

        Add(new CreateMetadataDataDto(
            "Social media influencer contact",
            ["contact@influencer.io"],
            ["+15551234567"],
            [
                new CreateMetadataDataDtoSocialLinks(new Uri("https://instagram.com/influencer"), "instagram-icon", "Instagram Page"),
                new CreateMetadataDataDtoSocialLinks(new Uri("https://tiktok.com/@influencer"), "tiktok-icon", "TikTok Profile"),
                new CreateMetadataDataDtoSocialLinks(new Uri("https://youtube.com/influencer"), "youtube-icon", "YouTube Channel")
            ]
        ));

        Add(new CreateMetadataDataDto(
            "Company directory entry",
            ["support@company.com", "info@company.com"],
            ["+18005551234", "+18005556789"],
            [
                new CreateMetadataDataDtoSocialLinks(new Uri("https://facebook.com/company"), "facebook-icon", "Facebook Page")
            ]
        ));

        Add(new CreateMetadataDataDto(
            "Freelancer portfolio",
            ["freelancer@portfolio.com"],
            ["+49123456789"],
            [
                new CreateMetadataDataDtoSocialLinks(new Uri("https://behance.net/freelancer"), "behance-icon", "Behance Portfolio"),
                new CreateMetadataDataDtoSocialLinks(new Uri("https://dribbble.com/freelancer"), "dribbble-icon", "Dribbble Portfolio")
            ]
        ));
    }
}

public class CreateMetadataInvalidTestData : TheoryData<CreateMetadataDataDto, string[]>
{
    public CreateMetadataInvalidTestData()
    {
        // 2️⃣ Case: Empty Description
        Add(new CreateMetadataDataDto(
            Description: "",
            Emails: ["user@example.com"],
            PhoneNumbers: ["+1234567890"],
            SocialLinks: []
        ), ["Description must not be empty."]);

        // 3️⃣ Case: Empty Emails List
        Add(new CreateMetadataDataDto(
            Description: "Valid Description",
            Emails: [],
            PhoneNumbers: ["+1234567890"],
            SocialLinks: []
        ), ["At least one email must be provided."]);

        // 4️⃣ Case: Invalid Email Format
        Add(new CreateMetadataDataDto(
            Description: "Valid Description",
            Emails: ["invalid-email"],
            PhoneNumbers: ["+1234567890"],
            SocialLinks: []
        ), ["Email is not valid."]);

        // 5️⃣ Case: Empty Email String
        Add(new CreateMetadataDataDto(
            Description: "Valid Description",
            Emails: [""],
            PhoneNumbers: ["+1234567890"],
            SocialLinks: []
        ), ["Email cannot be empty."]);

        // 6️⃣ Case: Empty Phone Numbers List
        Add(new CreateMetadataDataDto(
            Description: "Valid Description",
            Emails: ["user@example.com"],
            PhoneNumbers: [],
            SocialLinks: []
        ), ["At least one phone number must be provided."]);

        // 7️⃣ Case: Empty Phone Number String
        Add(new CreateMetadataDataDto(
            Description: "Valid Description",
            Emails: ["user@example.com"],
            PhoneNumbers: [""],
            SocialLinks: []
        ), ["Phone number cannot be empty."]);

        // 9️⃣ Case: Empty SocialLink Icon
        Add(new CreateMetadataDataDto(
            Description: "Valid Description",
            Emails: ["user@example.com"],
            PhoneNumbers: ["+1234567890"],
            SocialLinks:
            [
                new CreateMetadataDataDtoSocialLinks(
                    Url: new Uri("https://linkedin.com/user"),
                    Icon: "", // Empty Icon
                    Title: "LinkedIn"
                )
            ]
        ), ["SocialLink Icon is required."]);

        // 🔟 Case: Multiple Errors (Empty Description & Invalid Email)
        Add(new CreateMetadataDataDto(
            Description: "",
            Emails: ["invalid-email"],
            PhoneNumbers: ["+1234567890"],
            SocialLinks: []
        ),
        [
            "Description must not be empty.",
            "Email is not valid."
        ]);
    }
}

