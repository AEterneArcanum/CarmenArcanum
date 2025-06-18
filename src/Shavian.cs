namespace Arcane.Script;
/// <summary>
/// Functions related to the shavian alphabet.
/// </summary>
public static class Shavian
{
    /// <summary>
    /// Alphabet for potential future use.
    /// </summary>
    public static string[] Alphabet { get; } = [
        "𐑶",
        "𐑬",
        "𐑫",
        "𐑜",
        "𐑖",
        "𐑗",
        "𐑙",
        "𐑘",
        "𐑡",
        "𐑔",
        "𐑭",
        "𐑷",
        "𐑵",
        "𐑱",
        "𐑳",
        "𐑓",
        "𐑞",
        "𐑤",
        "𐑥",
        "𐑒",
        "𐑢",
        "𐑣",
        "𐑠",
        "𐑪",
        "𐑨",
        "𐑦",
        "𐑩",
        "𐑧",
        "𐑐",
        "𐑯",
        "𐑑",
        "𐑮",
        "𐑕",
        "𐑛",
        "𐑾",
        "𐑲",
        "𐑴",
        "𐑰",
        "𐑚",
        "𐑝",
        "𐑟",
        "𐑸",
        "𐑹",
        "𐑿",
        "𐑺",
        "𐑻",
        "𐑼",
        "𐑽",
    ];
    /// <summary>
    /// Punctuation symbols available on the shavian keyboard.
    /// </summary>
    public static char[] Punctuation { get; } = [
        '·',
        ',',
        '.',
        '/',
        '-',
        '=',
        '⸰',
        '+',
        '_',
        '/',
        '\'',
        ';',
        '%',
        '°',
        '‽',
        '€',
        '¥',
        '@',
        '*',
        '$',
        '£',
        '#',
        '…',
        '«',
        '»',
        ':',
        '\\',
        '"',
        '?',
        '›', // Special unicode is used due to existence on shavian keyboard.
        '‹', // Special unicode is used due to existence on shavian keyboard.
        '—',
        '–', // Special unicode is used due to existence on shavian keyboard.
        ')',
        '(',
        '^',
        '!'
    ];
    /// <summary>
    /// Check if char is on the list for shavian punctuation.
    /// </summary>
    /// <param name="c">Char to check</param>
    /// <returns>T/F</returns>
    public static bool IsPunctuation(char c) => Punctuation.Contains(c);
}
