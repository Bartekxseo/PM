namespace PM.Core.Exceptions
{
    /// <summary>The caller supplied input the domain cannot accept.</summary>
    public sealed class InvalidRequestException(string message) : DomainException(message);
}
