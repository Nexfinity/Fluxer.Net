namespace Fluxer.Net.Commands;

/// <summary>
/// The default pairs of opening and closing quotation marks used by <see cref="CommandServiceConfig"/>.
/// </summary>
internal static class QuotationAliasUtils
{
    /// <summary>
    /// The default map of opening to closing quotation marks, including regional and Unicode equivalents.
    /// </summary>
    /// <seealso cref="CommandServiceConfig.QuotationMarkAliasMap"/>
    internal static Dictionary<char, char> GetDefaultAliasMap
    {
        get
        {
            // Output of a gist provided by https://gist.github.com/ufcpp
            // https://gist.github.com/ufcpp/5b2cf9a9bf7d0b8743714a0b88f7edc5
            // This was not used for the implementation because of incompatibility with netstandard1.1
            return new Dictionary<char, char> {
                {'\"', '\"' },
                {'«', '»' },
                {'‘', '’' },
                {'“', '”' },
                {'„', '‟' },
                {'‹', '›' },
                {'‚', '‛' },
                {'《', '》' },
                {'〈', '〉' },
                {'「', '」' },
                {'『', '』' },
                {'〝', '〞' },
                {'﹁', '﹂' },
                {'﹃', '﹄' },
                {'＂', '＂' },
                {'＇', '＇' },
                {'｢', '｣' },
                {'(', ')' },
                {'༺', '༻' },
                {'༼', '༽' },
                {'᚛', '᚜' },
                {'⁅', '⁆' },
                {'⌈', '⌉' },
                {'⌊', '⌋' },
                {'❨', '❩' },
                {'❪', '❫' },
                {'❬', '❭' },
                {'❮', '❯' },
                {'❰', '❱' },
                {'❲', '❳' },
                {'❴', '❵' },
                {'⟅', '⟆' },
                {'⟦', '⟧' },
                {'⟨', '⟩' },
                {'⟪', '⟫' },
                {'⟬', '⟭' },
                {'⟮', '⟯' },
                {'⦃', '⦄' },
                {'⦅', '⦆' },
                {'⦇', '⦈' },
                {'⦉', '⦊' },
                {'⦋', '⦌' },
                {'⦍', '⦎' },
                {'⦏', '⦐' },
                {'⦑', '⦒' },
                {'⦓', '⦔' },
                {'⦕', '⦖' },
                {'⦗', '⦘' },
                {'⧘', '⧙' },
                {'⧚', '⧛' },
                {'⧼', '⧽' },
                {'⸂', '⸃' },
                {'⸄', '⸅' },
                {'⸉', '⸊' },
                {'⸌', '⸍' },
                {'⸜', '⸝' },
                {'⸠', '⸡' },
                {'⸢', '⸣' },
                {'⸤', '⸥' },
                {'⸦', '⸧' },
                {'⸨', '⸩' },
                {'【', '】'},
                {'〔', '〕' },
                {'〖', '〗' },
                {'〘', '〙' },
                {'〚', '〛' }
            };
        }
    }
}
