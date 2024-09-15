namespace MediaBedrock.Dolby.EncodingEngine;

public sealed class DolbyEncodingEngineException : Exception
{
    internal DolbyEncodingEngineException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}