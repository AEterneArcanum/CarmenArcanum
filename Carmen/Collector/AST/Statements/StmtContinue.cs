﻿using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Statements
{
    public record StmtContinue : Statement;

    public class StmtContinueParser : StatementParser
    {
        public StmtContinueParser() : base(StatementPriorities.Continue) { }
        public StmtContinueParser(int priority = StatementPriorities.Continue) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length == 1 && tokens[0].Type == TokenType.KeywordContinue)
            {
                result = new StmtContinue();
                return true;
            }
            return false;
        }
    }
}
