using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public record ExprAddressOf(Expression Expression) : Expression;

    public class ExprAddressOfParser : ExpressionParser
    {
        public ExprAddressOfParser():base(ExpressionPriorities.AddressOf) {}
        public ExprAddressOfParser(int priority = ExpressionPriorities.AddressOf) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? result)
        {
            result = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.OperatorTheAddressOf)
                return false;
            if (!Expression.TryParse(tokens[1..], out var expression))
                return false;
            result = new ExprAddressOf(expression!);
            return true;
        }
    }
}
