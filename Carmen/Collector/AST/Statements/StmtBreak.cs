using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Statements
{
    public record StmtBreak : Statement;

    public class StmtBreakParser : StatementParser
    {
        public StmtBreakParser() : base(StatementPriorities.Break) { }
        public StmtBreakParser(int priority = StatementPriorities.Break) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length == 1 && tokens[0].Type == TokenType.KeywordBreak)
            {
                result = new StmtBreak();
                return true;
            }
            return false;
        }
    }
}
