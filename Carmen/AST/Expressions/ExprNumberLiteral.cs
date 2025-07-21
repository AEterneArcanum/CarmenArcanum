using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprNumberLiteral(decimal Value) : Expression;

    public class ExprNumberLiteralParser : ExpressionParser
    {
        public ExprNumberLiteralParser(int priority = ExpressionPriorities.NumberLiteral) : base(priority) // Assuming lowest priority for number literals
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            if (!tokens.TryConvertToDecimal(out var numericValue))
            {
                expression = null;
                return false;
            }
            expression = new ExprNumberLiteral(numericValue);
            return true;
        }
    }
}
