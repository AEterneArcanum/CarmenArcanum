using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST
{
    public static class ExpressionEx
    {
        public static bool IsLiteral(this Expression expression)
        {
            return expression switch
            {
                Expressions.ExprStringLiteral or
                Expressions.ExprNumberLiteral or
                Expressions.ExprBooleanLiteral or
                Expressions.ExprNullLiteral or
                Expressions.ExprCharLiteral => true,
                _ => false,
            };
        }
        public static bool IsType(this Expression expression)
        {
            if (expression is not Expressions.ExprIdentifier)
            {
                return false;
            }
            var identifier = (Expressions.ExprIdentifier)expression;
            return identifier.Type == Expressions.IdentifierType.Type ||
                identifier.Type == Expressions.IdentifierType.Structure;
        }
    }

    public abstract record Expression : IVisitable
    {
        public static ExpressionHandler Handler { get; } = new ExpressionHandler();

        public static bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;

            if (tokens == null || tokens.Length == 0)
            {
                return false; // No tokens to parse
            }

            return Handler.TryParse(tokens, out expression);
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
