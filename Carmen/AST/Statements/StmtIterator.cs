using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtIterator(Expression Expression, Expression? IndexId, 
        Expression? ValueId, Statement? Body) : Statement
    {
    }

    public class StmtIteratorParser : StatementParser
    {
        public StmtIteratorParser(int priority = StatementPriorities.Loop) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordIterateOver)
                return false;
            // Find 'do' keyword
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordDo, out int index))
                return false;
            // Check for index and value identifiers
            bool namesIndex = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordWithIndexAs, out var idxIndex);
            bool namesValue = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordWithValueAs, out var valIndex);
            if (index < 2 || index >= tokens.Length - 1)
                return false;
            // ensure name index and value are before the 'do' keyword
            if (namesIndex && idxIndex >= index)
                return false;
            if (namesValue && valIndex >= index)
                return false;
            if (namesIndex)
            {
                // expression is betweeen 0 and idxIndex
                if (!Expression.TryParse(tokens[1..idxIndex], out var expression))
                    return false;
                if (namesValue)
                {
                    // value is between idxIndex and valIndex
                    if (!Expression.TryParse(tokens[(idxIndex + 1)..valIndex], out var valueId) ||
                        !Expression.TryParse(tokens[(valIndex + 1)..index], out var indexId))
                    {
                        return false;
                    }
                    // body is after the 'do' keyword
                    if (!Statement.TryParse(tokens[(index + 1)..], out var body))
                        return false;
                    result = new StmtIterator(expression!, indexId!, valueId!, body);
                }
                // value not named
                else
                {
                    if (!Expression.TryParse(tokens[(idxIndex + 1)..index], out var indexId) ||
                        !Statement.TryParse(tokens[(index + 1)..], out var body))
                    {
                        return false;
                    }
                    result = new StmtIterator(expression!, indexId!, null, body);
                }
            }
            else
            {
                // no index identifier
                if (namesValue)
                {
                    // expression is between 1 and valIndex
                    if (!Expression.TryParse(tokens[1..valIndex], out var expression) ||
                        !Expression.TryParse(tokens[(valIndex + 1)..index], out var valueId) ||
                        !Statement.TryParse(tokens[(index + 1)..], out var body))
                    {
                        return false;
                    }
                    result = new StmtIterator(expression!, null, valueId!, body);
                }
                else
                {
                    // expression is between 1 and index
                    if (!Expression.TryParse(tokens[1..index], out var expression) ||
                        !Statement.TryParse(tokens[(index + 1)..], out var body))
                    {
                        return false;
                    }
                    result = new StmtIterator(expression!, null, null, body);
                }
            }
            return true;
        }
    }
}
