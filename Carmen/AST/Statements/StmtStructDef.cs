using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtStructDef(Expression Id, Expression Definition) : Statement
    {
    }

    public class StmtStructDefParser : StatementParser
    {
        public StmtStructDefParser(int priority = StatementPriorities.Definitions) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = default;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordDefineStructure)
                return false;
            // Find with
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordWith, out var idxWith))
                return false;
            // Parse id
            if (!Expression.TryParse(tokens[1..idxWith], out var idExpression))
                return false;
            // Parse definition
            if (!Expression.TryParse(tokens[(idxWith + 1)..], out var defExpression))
                return false;
            result = new StmtStructDef(idExpression!, defExpression!);
            return true;
        }
    }
}
