using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtFuncDef(
        Expression Id,
        Expression? Struct,
        Expression? RetType,
        Expression? Parameters,
        Statement Body) : Statement;

    public class StmtFuncDefParser : StatementParser
    {
        public StmtFuncDefParser(int priority = StatementPriorities.Definitions) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordDefineFunction)
                return false;
            // Find Colon
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.PunctuationColon, out var idxColon))
                return false;
            if (!Statement.TryParse(tokens[(idxColon + 1)..], out var body))
                return false;

            // Is struct member
            bool structMember = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordFor, out var idxMember);

            // Defines return type
            bool retDefined = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordReturning, out var idxReturning);

            // defines parameters
            bool hasParameters = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordWith, out var idxWith);

            int staOfElement = 1;
            int endOfElement = idxColon;
            if (structMember) endOfElement = idxMember;
            else if (retDefined) endOfElement = idxReturning;
            else if (hasParameters) endOfElement = idxWith;

            // Parse func id
            if (!Expression.TryParse(tokens[staOfElement..endOfElement], out var funcId))
                return false;

            Expression? structEx = null;
            Expression? returnEx = null;
            Expression? parameEx = null;

            if (structMember) staOfElement = idxMember + 1;
            if (retDefined) endOfElement = idxReturning;
            else if (hasParameters) endOfElement = idxWith;
            else endOfElement = idxColon;

            // Parse struct id expression
            if (structMember)
            {
                if (!Expression.TryParse(tokens[staOfElement..endOfElement], out structEx))
                    return false;
            }
                

            if (retDefined) staOfElement = idxReturning + 1;
            if (hasParameters) endOfElement = idxWith; 
            else endOfElement = idxColon;

            if (retDefined)
            {
                if (!Expression.TryParse(tokens[staOfElement..endOfElement], out returnEx))
                    return false;
            }

            if (hasParameters) staOfElement = idxWith + 1;
            endOfElement = idxColon;

            if (hasParameters)
            {
                if (!Expression.TryParse(tokens[staOfElement..endOfElement], out parameEx))
                    return false;
            }

            result = new StmtFuncDef(funcId!, structEx, returnEx, parameEx, body!);
            return true;
        }
    }
}
