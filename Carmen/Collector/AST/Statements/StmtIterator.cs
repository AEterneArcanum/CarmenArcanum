using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST.Statements
{
    /// <summary>
    /// 'iterate over' EXPRESSION ('with index as' VARIABLE_ID)? ('with value as' VARIABLE_ID)? 'do' STATEMENT 
    /// # while in this loop provide 'index' and 'item' as keyword values. // compiler can ignore unused values in future.
    /// </summary>
    /// <param name="Expression"></param>
    /// <param name="IndexId"></param>
    /// <param name="ValueId"></param>
    /// <param name="Body"></param>
    public record StmtIterator(Expression Expression, ExprIdentifier? IndexId, 
        ExprIdentifier? ValueId, Statement? Body) : Statement;

    public class StmtIteratorParser : StatementParser
    {
        public StmtIteratorParser() : base (StatementPriorities.Loop) { }
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
                        valueId is not ExprIdentifier || ((ExprIdentifier)valueId).Type != IdentifierType.Variable ||
                        !Expression.TryParse(tokens[(valIndex + 1)..index], out var indexId) ||
                        indexId is not ExprIdentifier || ((ExprIdentifier)indexId).Type != IdentifierType.Variable)
                    {
                        return false;
                    }
                    // body is after the 'do' keyword
                    if (!Statement.TryParse(tokens[(index + 1)..], out var body))
                        return false;
                    result = new StmtIterator(expression!, (ExprIdentifier)indexId!, (ExprIdentifier)valueId!, body);
                }
                // value not named
                else
                {
                    if (!Expression.TryParse(tokens[(idxIndex + 1)..index], out var indexId) ||
                        indexId is not ExprIdentifier || ((ExprIdentifier)indexId).Type != IdentifierType.Variable ||
                        !Statement.TryParse(tokens[(index + 1)..], out var body))
                    {
                        return false;
                    }
                    result = new StmtIterator(expression!, (ExprIdentifier)indexId!, null, body);
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
                        valueId is not ExprIdentifier || ((ExprIdentifier)valueId).Type != IdentifierType.Variable ||
                        !Statement.TryParse(tokens[(index + 1)..], out var body))
                    {
                        return false;
                    }
                    result = new StmtIterator(expression!, null, (ExprIdentifier)valueId!, body);
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
