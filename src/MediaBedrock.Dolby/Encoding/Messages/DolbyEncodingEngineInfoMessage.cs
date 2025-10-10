namespace MediaBedrock.Dolby.Encoding.Messages;

public sealed record DolbyEncodingEngineInfoMessage : DolbyEncodingEngineMessage
{
    internal DolbyEncodingEngineInfoMessage(string message) : base(message)
    {
    }
}