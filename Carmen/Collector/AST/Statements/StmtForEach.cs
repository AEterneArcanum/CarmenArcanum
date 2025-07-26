using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Statements
{
    /// <summary>
    /// 'for each' VARIABLE_ID 'in' EXPRESSION 'do' STATEMENT
    /// </summary>
    /// <param name="Identifier"></param>
    /// <param name="Collection"></param>
    /// <param name="Body"></param>
    public record StmtForEach(
        ExprIdentifier Identifier, 
        Expression Collection, 
        Statement Body) 
        : Statement;

    public class StmtForEachParser : StatementParser
    {
        public StmtForEachParser() : base(StatementPriorities.Loop) { }
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
                identifier is not ExprIdentifier || ((ExprIdentifier)identifier).Type != IdentifierType.Variable ||
                !Expression.TryParse(tokens[(index + 1)..doIndex], out var collection) ||
                !Statement.TryParse(tokens[(doIndex + 1)..^1], out var body))
            {
                return false;
            }
            return true;
        }
    }
}
