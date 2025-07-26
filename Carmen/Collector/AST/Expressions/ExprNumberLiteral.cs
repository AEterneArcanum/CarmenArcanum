using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public record ExprNumberLiteral(decimal Value) : Expression;

    public class ExprNumberLiteralParser : ExpressionParser
    {
        public ExprNumberLiteralParser()
            :base(ExpressionPriorities.NumberLiteral) { }
        public ExprNumberLiteralParser(int priority = ExpressionPriorities.NumberLiteral) : base(priority) // Assuming lowest priority for number literals
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length != 1) return false;
            if (!decimal.TryParse(tokens[0].Raw, out var numericValue))
            {
                expression = null;
                return false;
            }
            expression = new ExprNumberLiteral(numericValue);
            return true;
        }
    }
}
