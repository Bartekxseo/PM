namespace PM.Core.Exceptions
{
    /// <summary>
    /// Base type for failures that are part of the domain's expected behaviour
    /// (bad input, missing resource, conflicting state) rather than defects.
    /// The API layer maps these to HTTP status codes; the domain itself stays
    /// unaware of HTTP.
    /// </summary>
    public abstract class DomainException(string message) : Exception(message);
}
