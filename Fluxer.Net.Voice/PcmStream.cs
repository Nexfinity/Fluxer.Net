namespace Fluxer.Net.Voice;

/// <summary>
/// A write only stream that sends 16 bit PCM bytes to a voice connection in frames of 20 milliseconds.
/// </summary>
internal sealed class PcmStream : Stream
{
    private const int FrameBytes = IAudioClient.SampleRate / 50 * IAudioClient.Channels * sizeof(short);

    private readonly IAudioClient _client;
    private readonly byte[] _frame = new byte[FrameBytes];
    private int _filled;

    public PcmStream(IAudioClient client)
    {
        _client = client;
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        while (!buffer.IsEmpty)
        {
            int count = Math.Min(buffer.Length, FrameBytes - _filled);
            buffer.Span[..count].CopyTo(_frame.AsSpan(_filled));
            _filled += count;
            buffer = buffer[count..];
            if (_filled == FrameBytes)
                await SendFrameAsync(FrameBytes, cancellationToken).ConfigureAwait(false);
        }
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        WriteAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

    public override void Write(byte[] buffer, int offset, int count) =>
        WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        int whole = _filled - _filled % (IAudioClient.Channels * sizeof(short));
        if (whole > 0)
            await SendFrameAsync(whole, cancellationToken).ConfigureAwait(false);
    }

    public override void Flush() => FlushAsync(CancellationToken.None).GetAwaiter().GetResult();

    public override async ValueTask DisposeAsync()
    {
        await FlushAsync(CancellationToken.None).ConfigureAwait(false);
        await base.DisposeAsync().ConfigureAwait(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _filled > 0)
            Flush();
        base.Dispose(disposing);
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    private async Task SendFrameAsync(int bytes, CancellationToken cancellationToken)
    {
        short[] samples = new short[bytes / sizeof(short)];
        Buffer.BlockCopy(_frame, 0, samples, 0, bytes);
        int leftover = _filled - bytes;
        if (leftover > 0)
            Buffer.BlockCopy(_frame, bytes, _frame, 0, leftover);
        _filled = leftover;
        await _client.SendAsync(samples, cancellationToken).ConfigureAwait(false);
    }
}
