using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprNullCoalesce(Expression Left, Expression Right) : Expression;

    public class ExprNullCoalesceParser : ExpressionParser
    {
        public ExprNullCoalesceParser(int priority = ExpressionPriorities.NullCoalescing) : base(priority) // Assuming lowest priority for null coalesce expressions
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens is null || tokens.Length < 3) return false; // Not enough tokens to form a null coalesce expression 
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorNullCoalesce, out int index)) return false; // No null coalesce operator found 
            if (!Expression.TryParse(tokens[..index], out var left) ||
                (left is not ExprIdentifier && left is not ExprArrayAccess && left is not ExprMemberAccess)) return false; // Failed to parse left expression 
            if (!Expression.TryParse(tokens[(index + 1)..], out var right)) return false; // Failed to parse right expression 
            expression = new ExprNullCoalesce(left!, right!);
            return true;
        }
    }
}
