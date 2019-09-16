using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lego.Ev3.Framework.Configuration
{
    /// <summary>
    /// Brick helper class for DI configuration
    /// </summary>
    public class BrickBuilder : IBrickBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrickBuilder"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public BrickBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        public IServiceCollection Services { get; }

    }
}
