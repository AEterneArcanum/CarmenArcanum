using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprBitwiseNot(Expression InternalExpression) : Expression;

    public class ExprBitwiseNotParser : ExpressionParser
    {
        public ExprBitwiseNotParser(int priority = ExpressionPriorities.BitwiseNot) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.OperatorBitwiseNot)
                return false;
            if (!Expression.TryParse(tokens[1..], out var innerExpression))
                return false;
            expression = new ExprBitwiseNot(innerExpression!);
            return true;
        }
    }
}
