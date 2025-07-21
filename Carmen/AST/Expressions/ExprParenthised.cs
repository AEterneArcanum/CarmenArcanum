using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprParenthised(Expression InternalExpression) : Expression;

    public class ExprParenthisedParser : ExpressionParser
    {
        public ExprParenthisedParser(int priority = ExpressionPriorities.Parenthised) 
            : base(priority) { }// Assuming lowest priority for parenthesized expressions

        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length < 2) return false; // Not enough tokens to form a parenthesized expression
            if (tokens[0].Type != TokenType.PunctuationOpenParenthesis || tokens[^1].Type != TokenType.PunctuationCloseParenthesis)
                return false; // Not a parenthesized expression
            if (!Expression.TryParse(tokens[1..^1], out var internalExpression)) return false; // Failed to parse the internal expression
            expression = new ExprParenthised(internalExpression!);
            return true; // Successfully parsed a parenthesized expression
        }
    }
}
