using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtLabel(Expressions.ExprIdentifier Identifier) : Statement
    {
    }

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
            var identifier = new Expressions.ExprIdentifier(tokens[0].Raw, Expressions.IdentifierType.Label);
            statement = new StmtLabel(identifier);
            return true;
        }
    }
}
