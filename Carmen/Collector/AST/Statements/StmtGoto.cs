using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST.Statements
{
    public record StmtGoto(ExprIdentifier Identifier) : Statement;

    public class StmtGotoParser : StatementParser
    {
        public StmtGotoParser() : base(StatementPriorities.Goto) { }
        public StmtGotoParser(int priority = StatementPriorities.Goto) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Statement? statement)
        {
            statement = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.KeywordGoto)
                return false;
            if (tokens[1].Type != TokenType.LabelIdentifier) return false;
            var identifier = new ExprIdentifier(tokens[1].Raw, IdentifierType.Label);
            statement = new StmtGoto(identifier);
            return true;
        }
    }
}
