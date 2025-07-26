using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Statements
{
    public record StmtFunctionCall(ExprFunctionCall FunctionCall) : Statement;

    public class StmtFunctionCallParser : StatementParser
    {
        public StmtFunctionCallParser() : base(StatementPriorities.Expression) { }
        public StmtFunctionCallParser(int priority = StatementPriorities.Expression) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            if (!Expression.TryParse(tokens, out var expr) 
                || expr is not ExprFunctionCall)
            {
                result = null;
                return false;
            }
            result = new StmtFunctionCall((ExprFunctionCall)expr!);
            return true;
        }
    }
}
