using Coderynx.Functional;

namespace MediaBedrock.Cli.Domain;

public sealed class DomainException(string errorCode, string message) : Exception(message)
{
    public string ErrorCode { get; } = errorCode;

    public static DomainException FromError(Error error)
    {
        return new DomainException(error.Code, error.Message);
    }
}