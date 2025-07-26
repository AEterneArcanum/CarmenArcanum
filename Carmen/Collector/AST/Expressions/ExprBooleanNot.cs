using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public record ExprBooleanNot(Expression Expression) : Expression;

    public class ExprBooleanNotParser : ExpressionParser
    {
        public ExprBooleanNotParser() : base(ExpressionPriorities.BooleanNot) { }
        public ExprBooleanNotParser(int priority = ExpressionPriorities.BooleanNot) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.KeywordNot)
                return false;
            if (!Expression.TryParse(tokens[1..], out var innerExpression))
                return false;
            expression = new ExprBooleanNot(innerExpression!);
            return true;
        }
    }
}
