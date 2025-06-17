/// <summary>
/// Functions related to the shavian alphabet.
/// </summary>
public static class Shavian
{
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
        '–', // Special unicode is used due to existence on shavian keyboard. // Criminally so does the regular minus sign ^^.
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
