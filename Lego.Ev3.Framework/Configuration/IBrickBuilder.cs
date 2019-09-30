namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Brick Builder Interface
    /// </summary>
    public interface IBrickBuilder
    {
        /// <summary>
        /// gets the IServiceCollection
        /// </summary>
        IServiceCollection Services { get; }
    }
}