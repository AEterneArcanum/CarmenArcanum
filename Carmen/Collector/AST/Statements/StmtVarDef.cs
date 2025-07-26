using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST.Statements
{
    public enum SuperType
    {
        Ordinary,
        Constant,
        Static
    }

    /// <summary>
    /// 'define' ( 'constant' VARIABLE_ID 'as' VARIABLE_TYPES 'equal to' EXPRESSION  # Required value 
    ///          | ('static')? VARIABLE_ID 'as' (('nullable')? VARIABLE_TYPES('equal to' EXPRESSION)?
    ///                                          | 'pointer to' VARIABLE_TYPES('equal to' EXPRESSION)?
    ///                                         )
    ///          )
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="SuperType"></param>
    /// <param name="Type"></param>
    /// <param name="Default"></param>
    /// <param name="Nullable"></param>
    /// <param name="Pointer"></param>
    public record StmtVarDef(
        ExprIdentifier Id,
        SuperType SuperType,
        ExprIdentifier Type,
        Expression? Default,
        bool Nullable,
        bool Pointer)
        : Statement;

    public class StmtVarDefParser : StatementParser
    {
        public StmtVarDefParser() : base(StatementPriorities.Definitions) { }
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
            int idStart = superType == SuperType.Ordinary ? 1 : 2;

            if (!Expression.TryParse(tokens[idStart..idxAs], out var idEx) // Id expression must validate to variable identifier.
                || idEx is not ExprIdentifier ||
                ((ExprIdentifier)idEx).Type != IdentifierType.Variable) return false;

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
            if (defInit) // type must validate to a valid type or struct identifier
            { 
                // form as to equals
                if (!Expression.TryParse(tokens[(idxAs + 1)..idxEquals], out type) ||
                    type is not ExprIdentifier || 
                    !((ExprIdentifier)type).Type.IsValueType()) return false;
            } 
            else 
            {
                // from as to end
                if (!Expression.TryParse(tokens[(idxAs + 1)..], out type) ||
                    type is not ExprIdentifier || 
                    !((ExprIdentifier)type).Type.IsValueType()) return false;
            }
            result = new StmtVarDef((ExprIdentifier)idEx!, superType, (ExprIdentifier)type!, defInitial, isNullable, isPointer);
            return true;
        }
    }
}
