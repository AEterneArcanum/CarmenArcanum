using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtWhile(Expression Expression, Statement Body)
        : Statement
    {
    }

    public class StmtWhileParser : StatementParser
    {
        public StmtWhileParser(int priority = StatementPriorities.Loop) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type == TokenType.KeywordWhile) return false;
            // Find do
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordDo, out var idxDo))
                return false;
            if (!Expression.TryParse(tokens[1..idxDo], out var expression)) return false;
            if (!Statement.TryParse(tokens[(idxDo + 1)..], out var body)) return false;
            result = new StmtWhile(expression!, body!);
            return true;
        }
    }
}
