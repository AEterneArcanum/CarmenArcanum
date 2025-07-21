using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Statements
{
    /// <summary>
    /// 'label' LABEL_ID
    /// </summary>
    /// <param name="Identifier"></param>
    public record StmtLabel(ExprIdentifier Identifier) : Statement;

    public class StmtLabelParser : StatementParser
    {
        public StmtLabelParser(int priority = StatementPriorities.Label) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? statement)
        {
            statement = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.LabelIdentifier)
                return false;
            var identifier = new ExprIdentifier(tokens[0].Raw, IdentifierType.Label);
            statement = new StmtLabel(identifier);
            return true;
        }
    }
}
