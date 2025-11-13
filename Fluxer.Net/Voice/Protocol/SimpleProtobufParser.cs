using System.Text;

namespace Fluxer.Net.Voice.Protocol;

/// <summary>
/// Simple protobuf parser for LiveKit messages.
/// Parses only what we need without using complex protobuf APIs.
/// </summary>
public static class SimpleProtobufParser
{
	/// <summary>
	/// Tries to extract SDP from a LiveKit protobuf message.
	/// LiveKit SignalResponse with Offer has structure:
	/// - Field 3 (offer) → SessionDescription
	/// - SessionDescription Field 2 (sdp) → string
	/// </summary>
	public static string? TryExtractSDP(byte[] data)
	{
		try
		{
			int pos = 0;
			while (pos < data.Length)
			{
				// Read tag
				if (!TryReadVarint(data, ref pos, out ulong tag))
					break;

				int fieldNumber = (int)(tag >> 3);
				int wireType = (int)(tag & 7);

				// Field 3 is "offer" in SignalResponse
				if (fieldNumber == 3 && wireType == 2) // 2 = LengthDelimited
				{
					if (!TryReadVarint(data, ref pos, out ulong length))
						break;

					int startPos = pos;
					int endPos = pos + (int)length;

					// Parse SessionDescription for SDP
					while (pos < endPos && pos < data.Length)
					{
						if (!TryReadVarint(data, ref pos, out ulong innerTag))
							break;

						int innerField = (int)(innerTag >> 3);
						int innerWire = (int)(innerTag & 7);

						// Field 2 is "sdp" in SessionDescription
						if (innerField == 2 && innerWire == 2)
						{
							if (!TryReadVarint(data, ref pos, out ulong sdpLength))
								break;

							if (pos + (int)sdpLength <= data.Length)
							{
								return Encoding.UTF8.GetString(data, pos, (int)sdpLength);
							}
						}
						else
						{
							// Skip field
							SkipField(data, ref pos, innerWire);
						}
					}

					pos = endPos;
				}
				else
				{
					// Skip field
					SkipField(data, ref pos, wireType);
				}
			}
		}
		catch
		{
			// Ignore parsing errors
		}

		return null;
	}

	/// <summary>
	/// Checks if the message is a JoinResponse (field 1 in SignalResponse).
	/// </summary>
	public static bool IsJoinResponse(byte[] data)
	{
		try
		{
			int pos = 0;
			while (pos < data.Length)
			{
				if (!TryReadVarint(data, ref pos, out ulong tag))
					break;

				int fieldNumber = (int)(tag >> 3);
				int wireType = (int)(tag & 7);

				// Field 1 is JoinResponse
				if (fieldNumber == 1)
					return true;

				SkipField(data, ref pos, wireType);
			}
		}
		catch
		{
			// Ignore errors
		}

		return false;
	}

	/// <summary>
	/// Tries to extract ICE candidate from a LiveKit protobuf message.
	/// LiveKit SignalResponse with Trickle has structure:
	/// - Field 4 (trickle) → TrickleRequest
	/// - TrickleRequest Field 1 (candidateInit) → string
	/// </summary>
	public static string? TryExtractIceCandidate(byte[] data)
	{
		try
		{
			int pos = 0;
			while (pos < data.Length)
			{
				// Read tag
				if (!TryReadVarint(data, ref pos, out ulong tag))
					break;

				int fieldNumber = (int)(tag >> 3);
				int wireType = (int)(tag & 7);

				// Field 4 is "trickle" in SignalResponse
				if (fieldNumber == 4 && wireType == 2) // 2 = LengthDelimited
				{
					if (!TryReadVarint(data, ref pos, out ulong length))
						break;

					int startPos = pos;
					int endPos = pos + (int)length;

					// Parse TrickleRequest for candidateInit
					while (pos < endPos && pos < data.Length)
					{
						if (!TryReadVarint(data, ref pos, out ulong innerTag))
							break;

						int innerField = (int)(innerTag >> 3);
						int innerWire = (int)(innerTag & 7);

						// Field 1 is "candidateInit" in TrickleRequest
						if (innerField == 1 && innerWire == 2)
						{
							if (!TryReadVarint(data, ref pos, out ulong candidateLength))
								break;

							if (pos + (int)candidateLength <= data.Length)
							{
								return Encoding.UTF8.GetString(data, pos, (int)candidateLength);
							}
						}
						else
						{
							// Skip field
							SkipField(data, ref pos, innerWire);
						}
					}

					pos = endPos;
				}
				else
				{
					// Skip field
					SkipField(data, ref pos, wireType);
				}
			}
		}
		catch
		{
			// Ignore parsing errors
		}

		return null;
	}

	/// <summary>
	/// Creates a protobuf SignalRequest with an Answer.
	/// Structure: Field 2 (answer) → SessionDescription { field 1 (type)="answer", field 2 (sdp)=sdpAnswer }
	/// </summary>
	public static byte[] CreateAnswerRequest(string sdpAnswer)
	{
		using var ms = new MemoryStream();

		// Calculate SessionDescription size first
		var typeBytes = Encoding.UTF8.GetBytes("answer");
		var sdpBytes = Encoding.UTF8.GetBytes(sdpAnswer);

		int sessionDescSize = 0;
		sessionDescSize += 1 + GetVarintSize((ulong)typeBytes.Length) + typeBytes.Length; // field 1
		sessionDescSize += 1 + GetVarintSize((ulong)sdpBytes.Length) + sdpBytes.Length;   // field 2

		// Write SignalRequest field 2 (answer)
		WriteTag(ms, 2, 2); // field 2, wire type LengthDelimited
		WriteVarint(ms, (ulong)sessionDescSize);

		// Write SessionDescription field 1 (type)
		WriteTag(ms, 1, 2);
		WriteVarint(ms, (ulong)typeBytes.Length);
		ms.Write(typeBytes, 0, typeBytes.Length);

		// Write SessionDescription field 2 (sdp)
		WriteTag(ms, 2, 2);
		WriteVarint(ms, (ulong)sdpBytes.Length);
		ms.Write(sdpBytes, 0, sdpBytes.Length);

		return ms.ToArray();
	}

	/// <summary>
	/// Creates a trickle request for sending ICE candidates.
	/// </summary>
	public static byte[] CreateTrickleRequest(string candidateInit, bool isPublisher = true)
	{
		using var ms = new MemoryStream();

		var candidateBytes = Encoding.UTF8.GetBytes(candidateInit);

		// Calculate TrickleRequest size
		int trickleSize = 0;
		trickleSize += 1 + GetVarintSize((ulong)candidateBytes.Length) + candidateBytes.Length; // field 1 (candidateInit)
		trickleSize += 2; // field 2 (target) - tag + varint 0 or 1

		// SignalRequest field 3 is trickle (TrickleRequest)
		WriteTag(ms, 3, 2); // field 3, wire type LengthDelimited
		WriteVarint(ms, (ulong)trickleSize);

		// Write TrickleRequest field 1 (candidateInit)
		WriteTag(ms, 1, 2);
		WriteVarint(ms, (ulong)candidateBytes.Length);
		ms.Write(candidateBytes, 0, candidateBytes.Length);

		// Write TrickleRequest field 2 (target: 0=publisher, 1=subscriber)
		WriteTag(ms, 2, 0); // wire type Varint
		WriteVarint(ms, (ulong)(isPublisher ? 0 : 1));

		return ms.ToArray();
	}

	/// <summary>
	/// Creates a ping request.
	/// </summary>
	public static byte[] CreatePingRequest(long timestamp)
	{
		using var ms = new MemoryStream();

		// SignalRequest field 13 is ping (int64)
		WriteTag(ms, 13, 0); // field 13, wire type Varint
		WriteVarint(ms, (ulong)timestamp);

		return ms.ToArray();
	}

	// Helper methods for protobuf encoding/decoding

	private static bool TryReadVarint(byte[] data, ref int pos, out ulong value)
	{
		value = 0;
		int shift = 0;

		while (pos < data.Length)
		{
			byte b = data[pos++];
			value |= (ulong)(b & 0x7F) << shift;

			if ((b & 0x80) == 0)
				return true;

			shift += 7;
			if (shift >= 64)
				return false;
		}

		return false;
	}

	private static void WriteVarint(Stream stream, ulong value)
	{
		while (value >= 0x80)
		{
			stream.WriteByte((byte)((value & 0x7F) | 0x80));
			value >>= 7;
		}
		stream.WriteByte((byte)value);
	}

	private static int GetVarintSize(ulong value)
	{
		int size = 1;
		while (value >= 0x80)
		{
			size++;
			value >>= 7;
		}
		return size;
	}

	private static void WriteTag(Stream stream, int fieldNumber, int wireType)
	{
		WriteVarint(stream, (ulong)((fieldNumber << 3) | wireType));
	}

	private static void SkipField(byte[] data, ref int pos, int wireType)
	{
		switch (wireType)
		{
			case 0: // Varint
				TryReadVarint(data, ref pos, out _);
				break;

			case 1: // Fixed64
				pos += 8;
				break;

			case 2: // LengthDelimited
				if (TryReadVarint(data, ref pos, out ulong length))
					pos += (int)length;
				break;

			case 5: // Fixed32
				pos += 4;
				break;

			default:
				throw new InvalidOperationException($"Unknown wire type: {wireType}");
		}
	}
}
