using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtGoto(Expressions.ExprIdentifier Identifier) : Statement
    {
    }

    public class StmtGotoParser : StatementParser
    {
        public StmtGotoParser(int priority = StatementPriorities.Goto) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Statement? statement)
        {
            statement = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.KeywordGoto)
                return false;
            if (tokens[1].Type != TokenType.LabelIdentifier) return false;
            var identifier = new Expressions.ExprIdentifier(tokens[1].Raw, Expressions.IdentifierType.Label);
            statement = new StmtGoto(identifier);
            return true;
        }
    }
}
