using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Expressions
{
    public enum RWPermission
    {
        ReadWrite,
        ReadOnly,
        WriteOnly,
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="Type"></param>
    /// <param name="Default"></param>
    /// <param name="Nullable"></param>
    /// <param name="Pointer"></param>
    /// <param name="Restricted"></param>
    /// <param name="RWPermission"></param>
    public record ExprFunctionParameter(
        ExprIdentifier Id,
        ExprIdentifier Type,
        Expression? Default,
        bool Nullable,
        bool Pointer,
        bool Restricted,
        RWPermission RWPermission) 
        : Expression;

    public class ExprFunctionParameterParser : ExpressionParser
    {
        public ExprFunctionParameterParser(int priority = ExpressionPriorities.FunctionParameter) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordDefine) return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordAs, out int idxAs)) return false;
            bool isNullable = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordNullable, out int idxNullable);
            bool isPointer = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordPointerTo, out int idxPointer);
            bool isRestrict = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordRestrict, out int idxRestrict);
            bool isOut = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordOut, out int idxOut);
            bool isIn = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordIn, out int idxIn);
            bool setsDefault = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordEqualTo, out int idxSet);
            // Check RW
            RWPermission rwPermit = RWPermission.ReadWrite;
            if (isOut && isIn) return false; // cant be both... sorta...
            else if (isOut)
                rwPermit = RWPermission.WriteOnly;
            else if (isIn)
                rwPermit = RWPermission.ReadOnly;

            if (!Expression.TryParse(tokens[1..idxAs], out var idExpr) ||
                idExpr is not ExprIdentifier || ((ExprIdentifier)idExpr).Type == IdentifierType.Function)
                return false;
            if (isNullable) idxAs++;
            if (isPointer) idxAs++;
            if (isRestrict) idxAs++;
            if (isOut) idxAs++;
            if (isIn) idxAs++;
            if (setsDefault) 
            {
                if (!Expression.TryParse(tokens[(idxAs + 1)..idxSet], out var typeExpr) ||
                    typeExpr is not ExprIdentifier || !((ExprIdentifier)typeExpr).Type.IsValueType())
                    return false;
                if (!Expression.TryParse(tokens[(idxSet + 1)..], out var setExpr))
                    return false;
                result = new ExprFunctionParameter((ExprIdentifier)idExpr!, (ExprIdentifier)typeExpr!, setExpr, isNullable, isPointer, isRestrict, rwPermit);
                return true;
            }
            else
            {
                if (!Expression.TryParse(tokens[(idxAs + 1)..], out var typeExpr) ||
                    typeExpr is not ExprIdentifier || !((ExprIdentifier)typeExpr).Type.IsValueType())
                    return false;
                result = new ExprFunctionParameter((ExprIdentifier)idExpr!, (ExprIdentifier)typeExpr!, null, isNullable, isPointer, isRestrict, rwPermit);
                return true;
            }
        }
    }
}
