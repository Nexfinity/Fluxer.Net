using NAudio.Wave;
using Serilog;

namespace Fluxer.Net.Voice;

/// <summary>
/// Plays audio files (MP3, WAV, etc.) through a voice connection.
/// Handles conversion to PCM and encoding to Opus format required by Discord.
/// </summary>
public class AudioPlayer : IDisposable
{
	private readonly VoiceClient _voiceClient;
	private readonly OpusEncoder _opusEncoder;
	private readonly ILogger? _logger;
	private bool _isPlaying;
	private CancellationTokenSource? _playCts;

	public event Action? OnPlaybackFinished;
	public event Action<Exception>? OnError;

	/// <summary>
	/// Creates a new AudioPlayer instance.
	/// </summary>
	/// <param name="voiceClient">The voice client to send audio through.</param>
	/// <param name="logger">Optional Serilog logger.</param>
	public AudioPlayer(VoiceClient voiceClient, ILogger? logger = null)
	{
		_voiceClient = voiceClient;
		_opusEncoder = new OpusEncoder();
		_logger = logger;
	}

	/// <summary>
	/// Plays an audio file through the voice connection.
	/// </summary>
	/// <param name="filePath">Path to the audio file (MP3, WAV, etc.).</param>
	/// <param name="cancellationToken">Optional cancellation token to stop playback.</param>
	public async Task PlayAsync(string filePath, CancellationToken cancellationToken = default)
	{
		if (_isPlaying)
		{
			_logger?.Warning("Already playing audio. Stop current playback first.");
			return;
		}

		if (!File.Exists(filePath))
		{
			var ex = new FileNotFoundException($"Audio file not found: {filePath}");
			_logger?.Error(ex, "Audio file not found");
			OnError?.Invoke(ex);
			return;
		}

		_playCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		_isPlaying = true;

		try
		{
			_logger?.Information("Starting playback of: {FilePath}", filePath);
			_voiceClient.SetSpeaking(true);

			await PlayAudioFileAsync(filePath, _playCts.Token);

			_logger?.Information("Playback finished: {FilePath}", filePath);
			_voiceClient.SetSpeaking(false);
			OnPlaybackFinished?.Invoke();
		}
		catch (OperationCanceledException)
		{
			_logger?.Information("Playback cancelled");
			_voiceClient.SetSpeaking(false);
		}
		catch (Exception ex)
		{
			_logger?.Error(ex, "Error during playback");
			_voiceClient.SetSpeaking(false);
			OnError?.Invoke(ex);
		}
		finally
		{
			_isPlaying = false;
			_playCts?.Dispose();
			_playCts = null;
		}
	}

	private async Task PlayAudioFileAsync(string filePath, CancellationToken cancellationToken)
	{
		using var reader = new AudioFileReader(filePath);

		// Resample to 48kHz stereo if needed
		var resampler = new MediaFoundationResampler(reader, new WaveFormat(48000, 2))
		{
			ResamplerQuality = 60 // High quality resampling
		};

		// Convert to PCM 16-bit
		var pcmStream = new WaveFloatTo16Provider(resampler);

		// Frame size for Opus (20ms at 48kHz, stereo, 16-bit = 3840 bytes)
		int frameSize = OpusEncoder.FrameSize;
		byte[] pcmBuffer = new byte[frameSize];
		byte[] opusBuffer = new byte[4000]; // Max Opus frame size

		// Calculate frame duration (20ms)
		int frameDurationMs = 20;
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		long frameCount = 0;

		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// Read PCM data
			int bytesRead = 0;
			int offset = 0;

			// Read a full frame (may require multiple Read calls)
			while (offset < frameSize)
			{
				int read = pcmStream.Read(pcmBuffer, offset, frameSize - offset);
				if (read == 0)
					break;

				bytesRead += read;
				offset += read;
			}

			// End of stream
			if (bytesRead == 0)
				break;

			// Pad with silence if we didn't get a full frame
			if (bytesRead < frameSize)
			{
				Array.Clear(pcmBuffer, bytesRead, frameSize - bytesRead);
			}

			// Encode to Opus
			int opusLength = _opusEncoder.Encode(pcmBuffer, frameSize, opusBuffer);

			// Send audio
			await _voiceClient.SendAudioAsync(opusBuffer, opusLength);

			// Timing control: sleep to maintain 20ms frame rate
			frameCount++;
			long expectedElapsedMs = frameCount * frameDurationMs;
			long actualElapsedMs = stopwatch.ElapsedMilliseconds;
			long sleepMs = expectedElapsedMs - actualElapsedMs;

			if (sleepMs > 0)
			{
				await Task.Delay((int)sleepMs, cancellationToken);
			}
		}
	}

	/// <summary>
	/// Stops the current playback.
	/// </summary>
	public void Stop()
	{
		if (_isPlaying)
		{
			_logger?.Information("Stopping playback");
			_playCts?.Cancel();
		}
	}

	/// <summary>
	/// Gets whether audio is currently playing.
	/// </summary>
	public bool IsPlaying => _isPlaying;

	public void Dispose()
	{
		Stop();
		_opusEncoder.Dispose();
		_playCts?.Dispose();
	}
}
