using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprNullCheck(Expression Expression, bool Not) : Expression;

    public class ExprNullCheckParser : ExpressionParser
    {
        public ExprNullCheckParser(int priority = ExpressionPriorities.NullCheck) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens == null || tokens.Length < 3)
            {
                return false; // Not enough tokens to parse a null check expression
            }
            if (tokens[^1].Type != TokenType.KeywordIsNull &&
                tokens[^1].Type != TokenType.KeywordIsNotNull)
            {
                return false; // Last token must be 'is null' or 'is not null'
            }
            // Determine if it's a null or not null check
            bool not = tokens[^1].Type == TokenType.KeywordIsNotNull;
            // Parse the expression
            if (!Expression.TryParse(tokens[..^1], out var expr) ||
                (expr is not ExprIdentifier && expr is not ExprArrayAccess && expr is not ExprMemberAccess))
            {
                return false; // Failed to parse the expression before 'is null' or 'is not null'
            }
            // Create the null check expression
            expression = new ExprNullCheck(expr!, not);
            return true;
        }
    }
}
