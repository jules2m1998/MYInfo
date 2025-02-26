namespace MYInfo.Domain.ValueObjects;

public class SocialLink
{
    public Uri Url { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
}
