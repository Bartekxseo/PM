namespace PM.Core.Exceptions
{
    /// <summary>A requested resource does not exist.</summary>
    public sealed class NotFoundException(string message) : DomainException(message);
}
