namespace Microsoft.Extensions.DependencyInjection
{
    public interface IBrickBuilder
    {
        IServiceCollection Services { get; }
    }
}