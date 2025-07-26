using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public record ExprTypeCheck(Expression Expression, ExprIdentifier Type) : Expression;

    public class ExprTypeCheckParser : ExpressionParser
    {
        public ExprTypeCheckParser():base(ExpressionPriorities.TypeCheck) { }
        public ExprTypeCheckParser(int priority = ExpressionPriorities.TypeCheck) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Expression? result)
        {
            result = null;
            if (tokens is null || tokens.Length < 3) return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordIsOfType, out int idxx))
                return false;
            if (idxx < 1 || idxx >= tokens.Length - 1) return false;
            if (!Expression.TryParse(tokens[..idxx], out Expression? expr))
                return false;
            if (!Expression.TryParse(tokens[(idxx + 1)..], out Expression? type) ||
                type is not ExprIdentifier || !((ExprIdentifier)type).IsType())
                return false;
            if (expr == null || type == null) return false;
            result = new ExprTypeCheck(expr, (ExprIdentifier)type);
            return true;
        }
    }
}