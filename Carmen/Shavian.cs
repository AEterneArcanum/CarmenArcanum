namespace Arcane.Carmen
{
    /// <summary>
    /// Functions and properties related to the Shavian alphabet.
    /// A generally terrible Alphabet, that does not follow its stated goals.
    /// </summary>
    public class Shavian
    {
        /// <summary>
        /// Alphabetic characters of the Shavian script.
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
        /// Punctuation characters used in the Shavian script.
        /// Available on the Shavian keyboard.
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
        /// Check if a character is part of the Shavian alphabet's punctuation.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns></returns>
        public static bool IsPunctuation(char c) => Punctuation.Contains(c);
        /// <summary>
        /// Check if a character is member to the Shavian alphabet.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns></returns>
        public static bool IsLetter(char c) => Alphabet.Any(x => x[0] == c);
    }
}
