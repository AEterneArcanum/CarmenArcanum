using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST.Statements
{
    /// <summary>
    /// 'label' LABEL_ID
    /// </summary>
    /// <param name="Identifier"></param>
    public record StmtLabel(ExprIdentifier Identifier) : Statement;

    public class StmtLabelParser : StatementParser
    {
        public StmtLabelParser() : base(StatementPriorities.Label) { }
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
