using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprIndex(Expression Index, bool FromLast, bool ZeroBased) : Expression;

    public class ExprIndexParser : ExpressionParser
    {
        public ExprIndexParser(int priority = ExpressionPriorities.IndexExpression) : base(priority) // Assuming lowest priority for index expressions
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            bool fromLast = false;
            bool zeroBased = true; // Default to zero-based indexing

            Token[] workingTokens = [];
            Array.Copy(tokens, new Token[tokens.Length], tokens.Length); // Create a copy of the tokens to work with

            if (workingTokens.Length < 2) return false;
            if (workingTokens.Length >= 3 && workingTokens[^1].Type == TokenType.KeywordFromLast)
            {
                fromLast = true;
                workingTokens = workingTokens[0..^1]; // Remove the "from last" keyword
            }
            if (workingTokens[0].Type != TokenType.KeywordIndex && workingTokens[^1].Type != TokenType.KeywordIndex) return false;
            zeroBased = workingTokens[0].Type == TokenType.KeywordIndex; // If the first token is "index", it is zero-based, otherwise it is one-based.
            if (!zeroBased)  // last token is 'index' check for final index and initial index
            {
                if (workingTokens[..^1].Length == 1 && workingTokens[0].Type == TokenType.KeywordInitial && !fromLast) // Initial Index
                {
                    expression = new ExprIndex(new ExprNumberLiteral(0), false, true); // Initial index is always zero-based
                }
                else if (workingTokens[..^1].Length == 1 && workingTokens[0].Type == TokenType.KeywordFinal && !fromLast) // Final Index
                {
                    expression = new ExprIndex(new ExprNumberLiteral(1), true, false); // Final index is always one-based
                }
                else // Initial / Final not in play
                {
                    if (!Expression.TryParse(workingTokens[..^1], out var idx)) return false;
                    expression = new ExprIndex(idx!, fromLast, zeroBased);
                }
            }
            else
            {
                if (!Expression.TryParse(workingTokens[1..], out var idx)) return false;
                expression = new ExprIndex(idx!, fromLast, zeroBased); // Index is always zero-based if the first token is "index"
            }
            return true;
        }
    }
}
