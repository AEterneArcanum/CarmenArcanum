using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public enum SuperType
    {
        Ordinary,
        Constant,
        Static
    }

    public record StmtVarDef(
        Expression Id,
        SuperType SuperType,
        Expression Type,
        Expression? Default,
        bool Nullable,
        bool Pointer)
        : Statement;

    public class StmtVarDefParser : StatementParser
    {
        public StmtVarDefParser(int priority = StatementPriorities.Definitions) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = default;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordDefine)
                return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordAs, out var idxAs)) return false;
            SuperType superType = tokens[1].Type switch
            {
                TokenType.KeywordConstant => SuperType.Constant,
                TokenType.KeywordStatic => SuperType.Static,
                _ => SuperType.Ordinary
            };
            int idStart = (superType == SuperType.Ordinary) ? 1 : 2;
            if (!Expression.TryParse(tokens[idStart..idxAs], out var idEx)) return false;
            bool isNullable = tokens[idxAs + 1].Type == TokenType.KeywordNullable;
            bool isPointer = tokens[idxAs + 1].Type == TokenType.KeywordPointer;
            bool defInit = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordEqualTo, out var idxEquals);
            Expression? defInitial = null;
            if (defInit)
            {
                if (!Expression.TryParse(tokens[(idxEquals + 1)..],  out defInitial)) 
                    return false;
            }
            Expression? type;
            if (defInit) 
            { 
                // form as to equals
                if (!Expression.TryParse(tokens[(idxAs + 1)..idxEquals], out type)) return false;
            } 
            else 
            {
                // from as to end
                if (!Expression.TryParse(tokens[(idxAs + 1)..], out type)) return false;
            }
            result = new StmtVarDef(idEx!, superType, type!, defInitial, isNullable, isPointer);
            return true;
        }
    }
}
