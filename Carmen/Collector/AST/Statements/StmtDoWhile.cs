using Arcane.Carmen.AST.Statements;
using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Statements
{
    public record StmtDoWhile(Expression Expression, Statement Statement) : Statement;

    public class StmtDoWhileParser : StatementParser
    {
        public StmtDoWhileParser() : base(StatementPriorities.Loop) { }
        public StmtDoWhileParser(int priority = StatementPriorities.Loop) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type == TokenType.KeywordDo) return false;
            // Find while
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordWhile, out var idxDo))
                return false;
            if (!Statement.TryParse(tokens[1..idxDo], out var body)) return false;
            if (!Expression.TryParse(tokens[(idxDo + 1)..], out var expression)) return false;
            result = new StmtWhile(expression!, body!);
            return true;
        }
    }
}
