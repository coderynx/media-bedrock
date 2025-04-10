namespace MediaBedrock.Dolby.EncodingEngine;

internal sealed record DolbyEncodingEngineLogLevel
{
    private const string InformationValue = "INFO";
    private const string InternalInformationValue = "INTERNAL_INFO";
    private const string WarningValue = "WARNING";
    private const string ErrorValue = "ERROR";

    public static readonly DolbyEncodingEngineLogLevel Information = new(InformationValue);
    public static readonly DolbyEncodingEngineLogLevel InternalInfo = new(InternalInformationValue);
    public static readonly DolbyEncodingEngineLogLevel Warning = new(WarningValue);
    public static readonly DolbyEncodingEngineLogLevel Error = new(ErrorValue);

    public DolbyEncodingEngineLogLevel(string level)
    {
        if (string.IsNullOrWhiteSpace(level))
        {
            throw new ArgumentException("Log level cannot be null or empty.", nameof(level));
        }

        var isInvalid = !level.Equals(InformationValue) &&
                        !level.Equals(InternalInformationValue) &&
                        !level.Equals(WarningValue) &&
                        !level.Equals(ErrorValue);
        if (isInvalid)
        {
            throw new ArgumentException($"Invalid log level: {level}", nameof(level));
        }

        Value = level.ToUpperInvariant();
    }

    public string Value { get; init; }

    public override string ToString()
    {
        return Value;
    }
}