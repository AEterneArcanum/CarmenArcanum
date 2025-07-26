using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public record ExprStringLiteral(string Value) : Expression;

    public class ExprStringLiteralParser : ExpressionParser
    {
        public ExprStringLiteralParser() : base(ExpressionPriorities.StringLiteral) { }
        public ExprStringLiteralParser(int priority = ExpressionPriorities.StringLiteral) : base(priority) // Assuming lowest priority for string literals
        {
        }

        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length != 1) return false;
            var token = tokens[0];
            if (token.Type != TokenType.LiteralString)
                return false;
            if (!token.Raw.StartsWith('"') || !token.Raw.EndsWith('"'))
                return false; // Ensure the string is properly quoted.
            string raw = token.Raw;
            if (raw.Length < 2)
                return false; // Empty string literal is not valid.
            raw = raw[1..^1]; // Remove the first and last character (the quotes).
            expression = new ExprStringLiteral(raw);
            return true;
        }

    }
}