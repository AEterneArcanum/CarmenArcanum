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

        public static string ToString(string s)
        {
            string r = string.Empty;
            foreach (char c in s)
            {
                r += ToAscii(c);
            }
            return r;
        }

        public static string ToAscii(char c)
        {
            return c.ToString() switch
            {
                "𐑶" => "oi",
                "𐑬" => "ou",
                "𐑫" => "oo",
                "𐑜" => "g",
                "𐑖" => "sh",
                "𐑗" => "ch",
                "𐑙" => "ng",
                "𐑘" => "y",
                "𐑡" => "dj",
                "𐑔" => "th",
                "𐑭" => "ah",
                "𐑷" => "aw",
                "𐑵" => "oo",
                "𐑱" => "a",
                "𐑳" => "u",
                "𐑓" => "f",
                "𐑞" => "th",
                "𐑤" => "l",
                "𐑥" => "m",
                "𐑒" => "k",
                "𐑢" => "w",
                "𐑣" => "h",
                "𐑠" => "zh",
                "𐑪" => "o",
                "𐑨" => "a",
                "𐑦" => "i",
                "𐑩" => "a",
                "𐑧" => "e",
                "𐑐" => "p",
                "𐑯" => "n",
                "𐑑" => "t",
                "𐑮" => "r",
                "𐑕" => "s",
                "𐑛" => "d",
                "𐑾" => "ia",
                "𐑲" => "i",
                "𐑴" => "oa",
                "𐑰" => "ea",
                "𐑚" => "b",
                "𐑝" => "v",
                "𐑟" => "z",
                "𐑸" => "ar",
                "𐑹" => "or",
                "𐑿" => "yoo",
                "𐑺" => "air",
                "𐑻" => "err",
                "𐑼" => "arr",
                "𐑽" => "ear",
                _ => c.ToString(),
            };
        }
    }
}
