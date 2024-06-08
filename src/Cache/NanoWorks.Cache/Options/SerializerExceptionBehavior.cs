namespace NanoWorks.Cache.Options
{
    /// <summary>
    /// Behavior for handling exceptions thrown by the serializer.
    /// </summary>
    public enum SerializerExceptionBehavior
    {
        /// <summary>
        /// Ignore the exception and continue.
        /// </summary>
        Ignore,

        /// <summary>
        /// Throw the exception.
        /// </summary>
        Throw,
    }
}