namespace PM.Core.Exceptions
{
    /// <summary>The request conflicts with the current state of the resource.</summary>
    public sealed class ConflictException(string message) : DomainException(message);
}
