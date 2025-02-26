using MYInfo.Domain.Abstractions;
using MYInfo.Domain.ValueObjects;

namespace MYInfo.Domain.Models;

public class UserMetaData : Entity<UserMetaDataId>
{
    public string Description { get; set; } = null!;
    public IReadOnlyCollection<string> Emails { get; set; } = null!;
    public IReadOnlyCollection<string> PhoneNumbers { get; set; } = null!;
    public ICollection<SocialLink> SocialLinks { get; } = [];
}