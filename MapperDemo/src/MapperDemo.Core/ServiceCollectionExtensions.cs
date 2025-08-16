using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MapperDemo.Core
{
    /// <summary>
    /// Extension methods for setting up mapping services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the mapping system services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddMappingSystem(this IServiceCollection services)
        {
            // Register the main handler
            services.AddSingleton<IMapHandler, MapHandler>();
            
            return services;
        }
        
        /// <summary>
        /// Adds all mappers from the specified assembly to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add mappers to.</param>
        /// <param name="assembly">The assembly to scan for mappers.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddMappersFromAssembly(
            this IServiceCollection services, Assembly assembly)
        {
            var mapperTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IMapper).IsAssignableFrom(t));

            foreach (var mapperType in mapperTypes)
            {
                services.AddSingleton(typeof(IMapper), mapperType);
            }

            return services;
        }
        
        /// <summary>
        /// Adds a specific mapper type to the service collection.
        /// </summary>
        /// <typeparam name="TMapper">The mapper type to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the mapper to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddMapper<TMapper>(this IServiceCollection services)
            where TMapper : class, IMapper
        {
            services.AddSingleton<IMapper, TMapper>();
            return services;
        }
    }
}