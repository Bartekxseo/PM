namespace PM.Core.Entities.Abstract
{
    public abstract class Entity<T>
    {
        public required T Id { get; set; }
    }
}
