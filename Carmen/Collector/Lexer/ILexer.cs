using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.Lexer
{
    public interface ILexer
    {
        /// <summary>
        /// Moving with index crawl across content spitting out tokens.
        /// </summary>
        /// <returns></returns>
        public Token[] Lex(string raw);
    }
}
