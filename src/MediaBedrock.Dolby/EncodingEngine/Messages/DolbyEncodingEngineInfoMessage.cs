namespace MediaBedrock.Dolby.EncodingEngine.Messages;

public sealed record DolbyEncodingEngineInfoMessage : DolbyEncodingEngineMessage
{
    internal DolbyEncodingEngineInfoMessage(string message) : base(message)
    {
    }
}