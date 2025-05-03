using System.Threading.Channels;
using MediaBedrock.Cli.Application.Jobs.Interfaces;

namespace MediaBedrock.Cli.Infrastructure.Messaging;

internal sealed class InMemoryMessageQueue
{
    private readonly Channel<IMessage> _channel = Channel.CreateUnbounded<IMessage>();
    public ChannelReader<IMessage> Reader => _channel.Reader;
    public ChannelWriter<IMessage> Writer => _channel.Writer;
}