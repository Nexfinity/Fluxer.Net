namespace Fluxer.Net.OAuth;

/// <summary>
/// Constants for the OAuth sign-in with Fluxer.
/// </summary>
public static class FluxerOAuthConstants
{
    /// <summary>
    /// The claim types that Fluxer adds to a signed-in user.
    /// </summary>
    public static class Claims
    {
        /// <summary>
        /// The hash of the user's avatar.
        /// </summary>
        public const string AvatarHash = "urn:fluxer:avatar:hash";

        /// <summary>
        /// The discriminator of the user.
        /// </summary>
        public const string Discriminator = "urn:fluxer:user:discriminator";

        /// <summary>
        /// The display name of the user.
        /// </summary>
        public const string DisplayName = "urn:fluxer:user:displayname";

        /// <summary>
        /// The flags of the user.
        /// </summary>
        public const string Flags = "urn:fluxer:user:flags";

        /// <summary>
        /// Whether the user's account is verified.
        /// </summary>
        public const string Verified = "urn:fluxer:user:verified";
    }
}
