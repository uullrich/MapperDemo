namespace MapperDemo.Core
{
    /// <summary>
    /// Exception thrown when mapping operations fail.
    /// </summary>
    public class MappingException : Exception
    {
        public MappingException(string message) : base(message)
        {
        }

        public MappingException(string message, Exception innerException) : base(message, innerException)
        {
        }
        
        public string? SourceType { get; set; }
        public string? TargetType { get; set; }
        
        public static MappingException CreateMappingNotFound(string sourceType, string targetType)
        {
            return new MappingException($"No mapper found for source type '{sourceType}' to target type '{targetType}'")
            {
                SourceType = sourceType,
                TargetType = targetType
            };
        }
        
        public static MappingException CreateInvalidSourceType(Type expectedType, Type actualType, string sourceType, string targetType)
        {
            return new MappingException($"Expected source of type '{expectedType.Name}' but got '{actualType.Name}'")
            {
                SourceType = sourceType,
                TargetType = targetType
            };
        }
    }
}