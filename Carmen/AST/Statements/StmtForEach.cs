using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtForEach(Expression Identifier, Expression Collection, Statement Body) : Statement
    {
        public override string ToString()
        {
            return $"foreach ({Identifier} in {Collection}) {{ {Body} }}";
        }
    }
    public class StmtForEachParser : StatementParser
    {
        public StmtForEachParser(int priority = StatementPriorities.Loop) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 6 || tokens[0].Type != TokenType.KeywordForEach)
                return false;
            // Find in keyword
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordIn, out int index))
                return false;
            // Find do keyword
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordDo, out int doIndex))
                return false;
            if (index < 1 || doIndex < index + 2 || doIndex >= tokens.Length - 1)
                return false;
            // Parse identifier
            if (!Expression.TryParse(tokens[1..index], out var identifier) ||
                !Expression.TryParse(tokens[(index + 1)..doIndex], out var collection) ||
                !Statement.TryParse(tokens[(doIndex + 1)..^1], out var body))
            {
                return false;
            }
            return true;
        }
    }
}
