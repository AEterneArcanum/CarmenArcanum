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
    /// <summary>
    /// FUNCTION_DEFINITION -->  'define function' FUNCTION_ID
    ///                                 ('for' STRUCTURE_ID)?
    ///                                 ('returning' VARIABLE_TYPES)?
    ///                                 ('with' FUNCTION_PARAMETERS)? # possible no parameters
    ///                                 ':' STATEMENT # either single statement or a block statement
    /// FUNCTION_PARAMETERS -->     ARRAYLITERAL << Of FUNCTION_PARAMETER
    ///                             | FUNCTION_PARAMETER
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="Struct"></param>
    /// <param name="RetType"></param>
    /// <param name="Parameters"></param>
    /// <param name="Body"></param>
    public record StmtFuncDef(
        ExprIdentifier Id,
        ExprIdentifier? Struct,
        ExprIdentifier? RetType,
        Expression? Parameters,
        Statement Body) : Statement;

    public class StmtFuncDefParser : StatementParser
    {
        public StmtFuncDefParser() : base(StatementPriorities.Definitions) { }
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
            if (!Expression.TryParse(tokens[staOfElement..endOfElement], out var funcId) ||
                funcId is not ExprIdentifier || ((ExprIdentifier)funcId).Type != IdentifierType.Function)
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
                if (!Expression.TryParse(tokens[staOfElement..endOfElement], out structEx) ||
                    structEx is not ExprIdentifier || ((ExprIdentifier)structEx).Type != IdentifierType.Structure)
                    return false;
            }
                

            if (retDefined) staOfElement = idxReturning + 1;
            if (hasParameters) endOfElement = idxWith; 
            else endOfElement = idxColon;

            if (retDefined)
            {
                if (!Expression.TryParse(tokens[staOfElement..endOfElement], out returnEx) ||
                    returnEx is not ExprIdentifier || !((ExprIdentifier)returnEx).Type.IsValueType())
                    return false;
            }

            if (hasParameters) staOfElement = idxWith + 1;
            endOfElement = idxColon;

            if (hasParameters)
            {
                if (!Expression.TryParse(tokens[staOfElement..endOfElement], out parameEx) ||
                    parameEx is not ExprFunctionParameter && parameEx is not ExprList)
                    return false;
                if (parameEx is ExprList lst)
                {
                    foreach (var item in lst.Expressions) 
                    { 
                        if (item is not ExprFunctionParameter) return false;
                    }
                }
            }

            result = new StmtFuncDef((ExprIdentifier)funcId!, (ExprIdentifier)structEx!, (ExprIdentifier)returnEx!, parameEx, body!);
            return true;
        }
    }
}
