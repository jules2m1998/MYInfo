namespace MYInfo.Application.Features.Metadata.Commands.CreateMetadata;


public record CreateMetadataDataDtoSocialLinks(Uri Url, string Icon, string Title);
public record CreateMetadataDataDto(string Description, IReadOnlyCollection<string> Emails, IReadOnlyCollection<string> PhoneNumbers, IReadOnlyCollection<CreateMetadataDataDtoSocialLinks> SocialLinks);
public record CreateMetadataCommandResult(Guid Id);
public record CreateMetadataCommand(CreateMetadataDataDto Dto) : ICommand<CreateMetadataCommandResult>;
public class CreateMetadataCommandValidator : AbstractValidator<CreateMetadataCommand>
{
    public CreateMetadataCommandValidator()
    {
        // Validate that the DTO itself is not null.
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage("Metadata data is required.");

        // Additional validations only if the DTO is provided.
        When(x => x.Dto != null, () =>
        {
            // Validate Description is not empty.
            RuleFor(x => x.Dto.Description)
                .NotEmpty()
                .WithMessage("Description must not be empty.");

            // Validate Emails: non-empty, and each email must be a valid email address.
            RuleFor(x => x.Dto.Emails)
                .NotEmpty()
                .WithMessage("At least one email must be provided.");

            RuleForEach(x => x.Dto.Emails)
                .NotEmpty()
                .WithMessage("Email cannot be empty.")
                .EmailAddress()
                .WithMessage("Email is not valid.");

            // Validate PhoneNumbers: non-empty, and no empty phone values.
            RuleFor(x => x.Dto.PhoneNumbers)
                .NotEmpty()
                .WithMessage("At least one phone number must be provided.");

            RuleForEach(x => x.Dto.PhoneNumbers)
                .NotEmpty()
                .WithMessage("Phone number cannot be empty.");

            // Validate each SocialLink (if any)
            RuleForEach(x => x.Dto.SocialLinks).ChildRules(socialLink =>
            {
                socialLink.RuleFor(link => link.Url)
                    .NotEmpty().WithMessage("SocialLink URL is required.");

                socialLink.RuleFor(link => link.Icon)
                    .NotEmpty().WithMessage("SocialLink Icon is required.");

                // Optionally check Title if needed; here Title is considered optional with a default.
            });
        });
    }
}