using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprConcatenate(Expression Left, Expression Right) : Expression
    {
        public override string ToString()
        {
            return $"{Left} + {Right}";
        }
    }

    public class ExprConcatenateParser : ExpressionParser
    {
        public ExprConcatenateParser(int priority = ExpressionPriorities.Concatenate) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length < 3)
                return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorConcatenatedWith, out int plusIndex))
                return false;
            if (plusIndex == 0 || plusIndex == tokens.Length - 1)
                return false; // Concatenation operator cannot be at the start or end of the expression.
            if (!Expression.TryParse(tokens[0..plusIndex], out var left))
            {
                return false; // Failed to parse the left expression.
            }
            if (!Expression.TryParse(tokens[(plusIndex + 1)..], out var right))
            {
                return false; // Failed to parse the right expression.
            }
            expression = new ExprConcatenate(left!, right!);
            return true; // Successfully parsed the concatenation expression.
        }
    }
}
