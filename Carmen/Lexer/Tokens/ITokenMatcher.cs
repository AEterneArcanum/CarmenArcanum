using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Lexer.Tokens
{
    /// <summary>
    /// Injectable token matching logic
    /// </summary>
    public interface ITokenMatcher
    {
        /// <summary>
        /// Attempt to match the raw string to a keyword token.
        /// </summary>
        /// <param name="raw">Raw string.</param>
        /// <param name="type">Identified type.</param>
        /// <returns>True if identified a keyword.</returns>
        public bool TryMatchKeyword(string raw, out TokenType type);
        /// <summary>
        /// Attempt to match the raw string to a complex token type. eg. decimal, string, char
        /// </summary>
        /// <param name="raw">Raw string.</param>
        /// <param name="type">Identified type.</param>
        /// <returns>True if identified a literal type.</returns>
        public bool TryMatchComplex(string raw, out TokenType type);
        /// <summary>
        /// Attempt to turn a string of numeric text tokens into a decimal value.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryConvertToDecimal(Token[] tokens, out decimal value);
    }
}
