using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtAlias(Expressions.ExprIdentifier Identifier, Expression Value) : Statement;

    public class StmtAliasParser : StatementParser
    {
        public StmtAliasParser(int priority = StatementPriorities.Alias) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordAlias)
                return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordAs, out int index))
                return false;
            if (index < 2 || index >= tokens.Length - 1)
                return false;
            if (!Expression.TryParse(tokens[1..index], out var identifier) || 
                identifier is not Expressions.ExprIdentifier || 
                ((Expressions.ExprIdentifier)identifier).Type != Expressions.IdentifierType.Alias ||
                !Expression.TryParse(tokens[(index + 1)..], out var value))
            {
                return false;
            }
            result = new StmtAlias((Expressions.ExprIdentifier)identifier!, value!);
            return true;
        }
    }
}
