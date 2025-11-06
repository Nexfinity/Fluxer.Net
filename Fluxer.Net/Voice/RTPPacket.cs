using System.Buffers.Binary;

namespace Fluxer.Net.Voice;

/// <summary>
/// Handles creation of RTP (Real-time Transport Protocol) packets for voice transmission.
/// Discord uses RTP for sending audio data over UDP.
/// </summary>
public class RTPPacket
{
	private const byte RTPVersion = 0x80; // RTP version 2
	private const byte RTPPayloadType = 0x78; // Opus payload type

	private ushort _sequence;
	private uint _timestamp;
	private readonly uint _ssrc;

	public RTPPacket(uint ssrc)
	{
		_ssrc = ssrc;
		_sequence = 0;
		_timestamp = 0;
	}

	/// <summary>
	/// Creates an RTP packet with the given Opus-encoded audio data.
	/// </summary>
	/// <param name="opusData">Opus-encoded audio data.</param>
	/// <param name="opusLength">Length of Opus data.</param>
	/// <returns>Complete RTP packet ready for transmission.</returns>
	public byte[] CreatePacket(byte[] opusData, int opusLength)
	{
		// RTP header is 12 bytes
		byte[] packet = new byte[12 + opusLength];

		// Byte 0: Version (2 bits) + Padding (1 bit) + Extension (1 bit) + CSRC count (4 bits)
		packet[0] = RTPVersion;

		// Byte 1: Marker (1 bit) + Payload type (7 bits)
		packet[1] = RTPPayloadType;

		// Bytes 2-3: Sequence number (big-endian)
		BinaryPrimitives.WriteUInt16BigEndian(packet.AsSpan(2), _sequence);

		// Bytes 4-7: Timestamp (big-endian)
		BinaryPrimitives.WriteUInt32BigEndian(packet.AsSpan(4), _timestamp);

		// Bytes 8-11: SSRC (big-endian)
		BinaryPrimitives.WriteUInt32BigEndian(packet.AsSpan(8), _ssrc);

		// Copy Opus data
		Buffer.BlockCopy(opusData, 0, packet, 12, opusLength);

		// Increment sequence and timestamp for next packet
		_sequence++;
		_timestamp += 960; // 20ms frame at 48kHz = 960 samples

		return packet;
	}

	/// <summary>
	/// Gets the current sequence number.
	/// </summary>
	public ushort Sequence => _sequence;

	/// <summary>
	/// Gets the current timestamp.
	/// </summary>
	public uint Timestamp => _timestamp;
}
