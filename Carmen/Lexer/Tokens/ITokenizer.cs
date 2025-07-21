using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Lexer.Tokens
{
    public interface ITokenizer
    {
        public List<Token> Tokenize(string content);
    }
}
