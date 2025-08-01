﻿using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Statements
{
    public record StmtAssert(Expression Condition, ExprStringLiteral? Message) : Statement;

    public class StmtAssertParser : StatementParser
    {
        public StmtAssertParser() : base(StatementPriorities.Assert) { }
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
                    !Expression.TryParse(tokens[(commaIndex + 1)..], out var message) ||
                    message is not ExprStringLiteral)
                    return false;
                statement = new StmtAssert(condition!, (ExprStringLiteral)message!);
            }
            return true;
        }
    }
}
