using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtAssert(Expression Condition, Expression? Message) : Statement
    {
        public override string ToString()
        {
            return $"assert {Condition} : {Message}";
        }
    }

    public class StmtAssertParser : StatementParser
    {
        public StmtAssertParser(int priority = StatementPriorities.Assert) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? statement)
        {
            statement = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.KeywordAssert)
                return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.PunctuationComma, out var commaIndex))
            {
                // no message
                if (!Expression.TryParse(tokens[1..], out var condition))
                    return false;
                statement = new StmtAssert(condition!, null);
            }
            else
            {
                // has message
                if (!Expression.TryParse(tokens[1..commaIndex], out var condition) ||
                    !Expression.TryParse(tokens[(commaIndex + 1)..], out var message))
                    return false;
                statement = new StmtAssert(condition!, message!);
            }
            return true;
        }
    }
}
