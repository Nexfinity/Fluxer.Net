using Concentus;
using Concentus.Structs;

namespace Fluxer.Net.Voice;

/// <summary>
/// Wrapper for Opus audio encoding.
/// Discord requires Opus-encoded audio at 48kHz, stereo, 20ms frames.
/// </summary>
public class OpusEncoder : IDisposable
{
	private readonly Concentus.Structs.OpusEncoder _encoder;
	private const int SampleRate = 48000;
	private const int Channels = 2;
	private const int FrameMillis = 20;
	private const int FrameSamples = SampleRate / 1000 * FrameMillis; // 960 samples per frame
	private const int FrameBytes = FrameSamples * Channels * 2; // 16-bit PCM = 2 bytes per sample

	public OpusEncoder()
	{
#pragma warning disable CS0618 // Type or member is obsolete
		_encoder = new Concentus.Structs.OpusEncoder(SampleRate, Channels, Concentus.Enums.OpusApplication.OPUS_APPLICATION_AUDIO);
#pragma warning restore CS0618 // Type or member is obsolete
	}

	/// <summary>
	/// Encodes PCM audio data to Opus format.
	/// </summary>
	/// <param name="pcmData">PCM audio data (16-bit, 48kHz, stereo).</param>
	/// <param name="pcmLength">Length of PCM data in bytes.</param>
	/// <param name="opusOutput">Output buffer for Opus-encoded data.</param>
	/// <returns>Number of bytes written to opusOutput.</returns>
	public int Encode(byte[] pcmData, int pcmLength, byte[] opusOutput)
	{
		// Convert bytes to shorts (16-bit PCM)
		short[] pcmShorts = new short[pcmLength / 2];
		Buffer.BlockCopy(pcmData, 0, pcmShorts, 0, pcmLength);

		// Encode to Opus using deprecated but functional method
#pragma warning disable CS0618 // Type or member is obsolete
		int encodedLength = _encoder.Encode(pcmShorts, 0, FrameSamples, opusOutput, 0, opusOutput.Length);
#pragma warning restore CS0618 // Type or member is obsolete
		return encodedLength;
	}

	/// <summary>
	/// Gets the required PCM frame size in bytes.
	/// </summary>
	public static int FrameSize => FrameBytes;

	public void Dispose()
	{
		// Concentus OpusEncoder doesn't require disposal, but we implement IDisposable for consistency
	}
}
