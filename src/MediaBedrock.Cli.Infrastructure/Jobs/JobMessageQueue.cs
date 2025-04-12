using System.Threading.Channels;
using MediaBedrock.Cli.Application.Jobs;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

internal sealed class JobMessageQueue
{
    private readonly Channel<JobMessage> _channel = Channel.CreateUnbounded<JobMessage>();
    public ChannelReader<JobMessage> Reader => _channel.Reader;
    public ChannelWriter<JobMessage> Writer => _channel.Writer;
}