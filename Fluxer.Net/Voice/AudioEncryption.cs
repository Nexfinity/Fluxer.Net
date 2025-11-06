using Sodium;

namespace Fluxer.Net.Voice;

/// <summary>
/// Handles encryption of voice packets using XSalsa20_Poly1305.
/// Discord requires all voice data to be encrypted.
/// </summary>
public static class AudioEncryption
{
	private const int NonceSize = 24;

	/// <summary>
	/// Encrypts an RTP packet using XSalsa20_Poly1305 encryption.
	/// </summary>
	/// <param name="rtpPacket">The RTP packet to encrypt (12-byte header + Opus data).</param>
	/// <param name="secretKey">The 32-byte secret key from Session Description.</param>
	/// <returns>Encrypted packet with nonce.</returns>
	public static byte[] Encrypt(byte[] rtpPacket, byte[] secretKey)
	{
		// Create nonce from RTP header (method: xsalsa20_poly1305)
		byte[] nonce = new byte[NonceSize];
		Buffer.BlockCopy(rtpPacket, 0, nonce, 0, 12); // Copy 12-byte RTP header as nonce

		// Encrypt the Opus data (everything after the 12-byte header)
		int opusDataLength = rtpPacket.Length - 12;
		byte[] opusData = new byte[opusDataLength];
		Buffer.BlockCopy(rtpPacket, 12, opusData, 0, opusDataLength);

		byte[] encrypted = SecretBox.Create(opusData, nonce, secretKey);

		// Create final packet: RTP header (12 bytes) + encrypted data
		byte[] finalPacket = new byte[12 + encrypted.Length];
		Buffer.BlockCopy(rtpPacket, 0, finalPacket, 0, 12); // Copy RTP header
		Buffer.BlockCopy(encrypted, 0, finalPacket, 12, encrypted.Length); // Copy encrypted data

		return finalPacket;
	}

	/// <summary>
	/// Encrypts an RTP packet using XSalsa20_Poly1305_Suffix encryption mode.
	/// </summary>
	/// <param name="rtpPacket">The RTP packet to encrypt.</param>
	/// <param name="secretKey">The 32-byte secret key.</param>
	/// <returns>Encrypted packet with nonce suffix.</returns>
	public static byte[] EncryptWithSuffix(byte[] rtpPacket, byte[] secretKey)
	{
		// Generate random nonce
		byte[] nonce = SodiumCore.GetRandomBytes(NonceSize);

		// Encrypt the Opus data
		int opusDataLength = rtpPacket.Length - 12;
		byte[] opusData = new byte[opusDataLength];
		Buffer.BlockCopy(rtpPacket, 12, opusData, 0, opusDataLength);

		byte[] encrypted = SecretBox.Create(opusData, nonce, secretKey);

		// Create final packet: RTP header + encrypted data + nonce suffix
		byte[] finalPacket = new byte[12 + encrypted.Length + NonceSize];
		Buffer.BlockCopy(rtpPacket, 0, finalPacket, 0, 12); // RTP header
		Buffer.BlockCopy(encrypted, 0, finalPacket, 12, encrypted.Length); // Encrypted data
		Buffer.BlockCopy(nonce, 0, finalPacket, 12 + encrypted.Length, NonceSize); // Nonce suffix

		return finalPacket;
	}

	/// <summary>
	/// Encrypts an RTP packet using XSalsa20_Poly1305_Lite encryption mode.
	/// </summary>
	/// <param name="rtpPacket">The RTP packet to encrypt.</param>
	/// <param name="secretKey">The 32-byte secret key.</param>
	/// <param name="nonceCounter">Counter for generating nonce (incremented after each use).</param>
	/// <returns>Encrypted packet with nonce suffix (4 bytes).</returns>
	public static byte[] EncryptWithLite(byte[] rtpPacket, byte[] secretKey, ref uint nonceCounter)
	{
		// Generate nonce from counter
		byte[] nonce = new byte[NonceSize];
		byte[] counterBytes = BitConverter.GetBytes(nonceCounter);
		Buffer.BlockCopy(counterBytes, 0, nonce, 0, 4);

		// Encrypt the Opus data
		int opusDataLength = rtpPacket.Length - 12;
		byte[] opusData = new byte[opusDataLength];
		Buffer.BlockCopy(rtpPacket, 12, opusData, 0, opusDataLength);

		byte[] encrypted = SecretBox.Create(opusData, nonce, secretKey);

		// Create final packet: RTP header + encrypted data + 4-byte nonce
		byte[] finalPacket = new byte[12 + encrypted.Length + 4];
		Buffer.BlockCopy(rtpPacket, 0, finalPacket, 0, 12); // RTP header
		Buffer.BlockCopy(encrypted, 0, finalPacket, 12, encrypted.Length); // Encrypted data
		Buffer.BlockCopy(counterBytes, 0, finalPacket, 12 + encrypted.Length, 4); // Nonce (4 bytes)

		nonceCounter++;
		return finalPacket;
	}
}
