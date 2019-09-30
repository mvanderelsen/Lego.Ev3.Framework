using Lego.Ev3.Framework;
using Lego.Ev3.Framework.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class BrickBuilderServiceCollectionExtensions
    {
        /// <summary>
        /// Creates a builder
        /// </summary>
        /// <param name="services">The services</param>
        /// <returns></returns>
        public static IServiceCollection AddBrick(this IServiceCollection services)
        {
            BrickBuilder builder = new BrickBuilder(services);
            builder.Services.AddOptions();
            builder.Services.AddSingleton(r => r.GetRequiredService<IOptions<BrickOptions>>().Value);
            builder.Services.AddSingleton<Brick>();
            return builder.Services;
        }

        /// <summary>
        /// Adds Brick
        /// </summary>
        /// <param name="services">The services</param>
        /// <param name="setupAction">The setup action</param>
        /// <returns></returns>
        public static IServiceCollection AddBrick(this IServiceCollection services, Action<BrickOptions> setupAction)
        {
            services.Configure(setupAction);
            return services.AddBrick();
        }

        /// <summary>
        /// Adds the Brick
        /// </summary>
        /// <param name="services">The services</param>
        /// <param name="configuration">The configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddBrick(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BrickOptions>(configuration.GetSection("Brick"));
            return services.AddBrick();
        }
    }
}
