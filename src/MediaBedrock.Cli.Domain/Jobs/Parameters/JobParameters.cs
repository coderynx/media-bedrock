namespace MediaBedrock.Cli.Domain.Jobs.Parameters;

/// <summary>
///     Represents the parameters required for a job, including inputs, outputs, and properties.
/// </summary>
/// <param name="Inputs">An array of input parameters for the job.</param>
/// <param name="Outputs">An array of output parameters for the job.</param>
/// <param name="Properties">An array of property parameters for the job.</param>
public sealed record JobParameters(
    JobInputParameter[] Inputs,
    JobOutputParameter[] Outputs,
    JobPropertyParameter[] Properties);