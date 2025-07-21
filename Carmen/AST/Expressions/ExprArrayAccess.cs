using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprArrayAccess(Expression Accessed, Expression Index) : Expression;

    public class ExprArrayAccessParser : ExpressionParser
    {
        public ExprArrayAccessParser(int priority = ExpressionPriorities.ArrayAccess) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null!;
            if (tokens.Length < 5) return false; 
            if (tokens[0].Type != TokenType.KeywordThe) return false; 
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordOf, out int idxOf) || idxOf < 1 || idxOf > tokens.Length - 2) return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordIndex, out int idxIndex) || idxIndex < 1 || idxIndex >= idxOf) return false; 
            if (!Expression.TryParse(tokens[1..idxOf], out var index) || index is not ExprIndex) return false;
            if (!Expression.TryParse(tokens[(idxOf + 1)..], out var identifier)) return false;
            expression = new ExprArrayAccess(identifier!, index);
            return true;
        }
    }
}
