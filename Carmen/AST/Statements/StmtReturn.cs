using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtReturn(Expression? Value) : Statement
    {
        public override string ToString()
        {
            return Value is null ? "return;" : $"return {Value};";
        }
    }

    public class StmtReturnParser : StatementParser
    {
        public StmtReturnParser(int priority = StatementPriorities.Return) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Statement? statement)
        {
            // Check for initial 'return' keyword
            statement = null;
            if (tokens.Length == 0) return false;
            if (tokens[0].Type != TokenType.KeywordReturn) return false;
            if (tokens.Length == 1)
            {
                // If only 'return' is present, it's a return without a value
                statement = new StmtReturn(null);
                return true;
            }
            if (!Expression.TryParse(tokens[1..], out var expression)) return false;
            statement = new StmtReturn(expression);
            return true;
        }
    }
}
