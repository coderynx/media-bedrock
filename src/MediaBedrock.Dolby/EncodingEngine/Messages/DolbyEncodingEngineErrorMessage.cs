namespace MediaBedrock.Dolby.EncodingEngine.Messages;

public sealed record DolbyEncodingEngineErrorMessage : DolbyEncodingEngineMessage
{
    internal DolbyEncodingEngineErrorMessage(string message) : base(message)
    {
    }
}