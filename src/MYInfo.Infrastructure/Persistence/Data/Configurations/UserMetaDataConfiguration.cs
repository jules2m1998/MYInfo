using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MYInfo.Domain.Models;
using MYInfo.Domain.ValueObjects;
using System.Text.Json;

namespace MYInfo.Infrastructure.Persistence.Data.Configurations;

public class UserMetaDataConfiguration : IEntityTypeConfiguration<UserMetaData>
{
    public void Configure(EntityTypeBuilder<UserMetaData> builder)
    {
        builder.HasKey(u => u.Id);
        builder
            .Property(u => u.Id)
            .HasConversion(orderId => orderId.Value, dbId => UserMetaDataId.Of(dbId));
        var options = new JsonSerializerOptions();
        builder.Property(u => u.SocialLinks)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<ICollection<SocialLink>>(v, options) ?? new List<SocialLink>()
                );
        builder.Property(u => u.Emails)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<string[]>(v, options) ?? Array.Empty<string>()
                );
        builder.Property(u => u.PhoneNumbers)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<string[]>(v, options) ?? Array.Empty<string>()
                );
    }
}
