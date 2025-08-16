namespace MapperDemo.Core
{
    /// <summary>
    /// Base implementation for strongly-typed mappers.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TTarget">The target type.</typeparam>
    public abstract class BaseMapper<TSource, TTarget> : IMapper, ITypeMapper<TSource, TTarget>
    {
        protected readonly string SourceTypeName;
        protected readonly string TargetTypeName;

        protected BaseMapper(string sourceTypeName, string targetTypeName)
        {
            SourceTypeName = sourceTypeName ?? throw new ArgumentNullException(nameof(sourceTypeName));
            TargetTypeName = targetTypeName ?? throw new ArgumentNullException(nameof(targetTypeName));
        }

        /// <inheritdoc />
        public bool CanMap(string sourceType, string targetType)
        {
            return string.Equals(sourceType, SourceTypeName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(targetType, TargetTypeName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public object Map(object source, string sourceType, string targetType)
        {
            if (!CanMap(sourceType, targetType))
            {
                throw new MappingException(
                    $"This mapper does not support mapping from '{sourceType}' to '{targetType}'. " +
                    $"It supports mapping from '{SourceTypeName}' to '{TargetTypeName}'.");
            }

            if (!(source is TSource typedSource))
            {
                throw MappingException.CreateInvalidSourceType(
                    typeof(TSource), source.GetType(), sourceType, targetType);
            }

            var result = Map(typedSource);
            if (result is null)
            {
                throw new MappingException(
                    $"Mapping from '{SourceTypeName}' to '{TargetTypeName}' returned null.");
            }
            return result;
        }

        /// <inheritdoc />
        public abstract TTarget Map(TSource source);
    }
}