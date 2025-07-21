using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprArraySlice(Expression Array, Expression? Start, Expression? End) : Expression;

    public class ExprArraySliceParser : ExpressionParser
    {
        public ExprArraySliceParser(int priority = ExpressionPriorities.ArraySlice) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            // Minimum length check
            expression = null!;
            if (tokens == null) return false;
            if (tokens.Length < 5) return false;
            // Ensure first token is 'elements'
            if (tokens[0].Type != TokenType.KeywordElements) return false;
            // Check is second token is 'beginning at' or 'ending at'
            bool unary = tokens[1].Type == TokenType.KeywordBeginningAt || tokens[1].Type == TokenType.KeywordEndingAt;
            // Find top level index of 'of'
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordOf, out int ofIndex))
            {
                return false;
            }
            // if binary find top level index of 'through' that must be before 'of'
            if (!unary)
            {
                if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordThrough, out int throughIndex) || throughIndex >= ofIndex)
                {
                    return false;
                }
                // Get expression between elements and through
                if (!Expression.TryParse(tokens[1..throughIndex], out var start))
                {
                    return false;
                }
                // Get expression between through and of
                if (!Expression.TryParse(tokens[(throughIndex + 1)..ofIndex], out var end))
                {
                    return false;
                }
                // Get expression after of
                if (!Expression.TryParse(tokens[(ofIndex + 1)..], out var array))
                {
                    return false;
                }
                expression = new ExprArraySlice(array, start, end);
                return true;
            }
            bool isBeginningAt = tokens[1].Type == TokenType.KeywordBeginningAt;
            // If unary get token between 'beginningAt' or 'endingAt' and 'of'
            if (!Expression.TryParse(tokens[2..ofIndex], out var startUnary))
            {
                return false;
            }
            // Get expression after of
            if (!Expression.TryParse(tokens[(ofIndex + 1)..], out var arrayUnary))
            {
                return false;
            }
            if (isBeginningAt)
            {
                expression = new ExprArraySlice(arrayUnary!, startUnary, null);
            }
            else
            {
                expression = new ExprArraySlice(arrayUnary!, null, startUnary);
            }
            return true;
        }
    }
}
