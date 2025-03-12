namespace MYInfo.Domain.ValueObjects;

public class UserMetaDataId
{
    public Guid Value { get; }

    private UserMetaDataId(Guid value) => Value = value;

    public static UserMetaDataId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{nameof(UserMetaDataId)} cannot be empty");
        }

        return new(value);
    }
}
