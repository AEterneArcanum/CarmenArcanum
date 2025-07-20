using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Lexer.Tokens
{
    public record Token(TokenType Type, string Raw, int Line, int Column);
}
