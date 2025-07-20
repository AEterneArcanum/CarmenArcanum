using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprTypeCheck(Expression Expression, Expression Type) : Expression;

    public class ExprTypeCheckParser : ExpressionParser
    {
        public ExprTypeCheckParser(int priority) : base(priority)
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
            if (!Expression.TryParse(tokens[(idxx + 1)..], out Expression? type))
                return false;
            if (expr == null || type == null) return false;
            result = new ExprTypeCheck(expr, type);
            return true;
        }
    }
}