namespace MYInfo.Domain.Exceptions;

public class DomainException : Exception
{
    // Parameterless constructor
    public DomainException()
    {
    }

    // Constructor with a message parameter
    public DomainException(string message)
        : base($"Domain Exception: \"{message}\" throws from Domain Layer")
    {
    }

    // Constructor with a message and inner exception
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

