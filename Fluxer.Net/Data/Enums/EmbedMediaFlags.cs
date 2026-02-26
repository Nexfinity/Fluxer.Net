namespace Fluxer.Net.Data.Enums;

/// <remarks>
/// <see href="https://github.com/fluxerapp/fluxer/blob/848269a4d4df7349acfc861ff926b17fe4c4a548/packages/constants/src/ChannelConstants.tsx#L157"/>
/// </remarks>
[Flags]
public enum EmbedMediaFlags
{
    None = 0,

    ContainsExplicitMedia = 1 << 4,

    IsAnimated = 1 << 5
}
