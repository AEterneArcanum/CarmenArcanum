using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Statements
{
    /// <summary>
    /// 'using' VARIABLE_ID 'as' EXPRESSION
    /// For defining or instancing must be cleaned at end of block
    /// </summary>
    /// <param name="Identifier"></param>
    /// <param name="Value"></param>
    public record StmtUsing(ExprIdentifier Identifier, Expression Value) : Statement;

    public class StmtUsingParser : StatementParser
    {
        public StmtUsingParser(int priority = StatementPriorities.Using) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordUsing)
                return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordAs, out int index))
                return false;
            if (index < 2 || index >= tokens.Length - 1)
                return false;
            if (!Expression.TryParse(tokens[1..index], out var identifier) ||
                identifier is not ExprIdentifier || ((ExprIdentifier)identifier).Type != IdentifierType.Variable ||
                !Expression.TryParse(tokens[(index + 1)..], out var value))
            {
                return false;
            }
            result = new StmtUsing((ExprIdentifier)identifier!, value!);
            return true;
        }
    }
}
