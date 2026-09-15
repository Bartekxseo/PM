namespace PM.Core.Exceptions
{
    /// <summary>
    /// The request is valid but cannot be served right now because a finite
    /// resource is exhausted. Retrying later may succeed.
    /// </summary>
    public sealed class CapacityExceededException(string message) : DomainException(message);
}
