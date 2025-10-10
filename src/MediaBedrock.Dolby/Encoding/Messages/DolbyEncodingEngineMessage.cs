namespace MediaBedrock.Dolby.Encoding.Messages;

public abstract record DolbyEncodingEngineMessage
{
    private protected DolbyEncodingEngineMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));
        }

        Message = message;
    }

    public string Message { get; protected set; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;

    public override string ToString()
    {
        return Message;
    }
}