using Microsoft.Extensions.Logging;

namespace MapperDemo.Core
{
    /// <summary>
    /// Main implementation of the mapping handler that orchestrates the mapping operations.
    /// </summary>
    public class MapHandler : IMapHandler
    {
        private readonly IEnumerable<IMapper> _mappers;
        private readonly ILogger<MapHandler>? _logger;

        public MapHandler(IEnumerable<IMapper> mappers, ILogger<MapHandler>? logger = null)
        {
            _mappers = mappers ?? throw new ArgumentNullException(nameof(mappers));
            _logger = logger;
        }

        /// <inheritdoc />
        public object Map(object data, string sourceType, string targetType)
        {
            ValidateInputs(data, sourceType, targetType);
            
            _logger?.LogDebug("Mapping request from {SourceType} to {TargetType}", sourceType, targetType);
            
            var mapper = _mappers.FirstOrDefault(m => m.CanMap(sourceType, targetType));
            
            if (mapper == null)
            {
                _logger?.LogError("No mapper found for {SourceType} to {TargetType}", sourceType, targetType);
                throw MappingException.CreateMappingNotFound(sourceType, targetType);
            }
            
            try
            {
                var result = mapper.Map(data, sourceType, targetType);
                _logger?.LogDebug("Successfully mapped {SourceType} to {TargetType}", sourceType, targetType);
                return result;
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                _logger?.LogError(ex, "Error mapping from {SourceType} to {TargetType}", sourceType, targetType);
                throw new MappingException($"Error mapping from '{sourceType}' to '{targetType}'", ex)
                {
                    SourceType = sourceType,
                    TargetType = targetType
                };
            }
        }

        /// <inheritdoc />
        public TTarget Map<TTarget>(object data, string sourceType, string targetType)
        {
            var result = Map(data, sourceType, targetType);
            
            if (result is TTarget typedResult)
            {
                return typedResult;
            }
            
            throw new MappingException($"The mapper returned a result of type '{result?.GetType().Name ?? "null"}' " +
                                       $"which cannot be cast to the requested type '{typeof(TTarget).Name}'");
        }
        
        private void ValidateInputs(object data, string sourceType, string targetType)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            
            if (string.IsNullOrWhiteSpace(sourceType))
                throw new ArgumentException("Source type cannot be null or empty", nameof(sourceType));
            
            if (string.IsNullOrWhiteSpace(targetType))
                throw new ArgumentException("Target type cannot be null or empty", nameof(targetType));
        }
    }
}