using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Expressions
{
    public enum BitDirection
    {
        Left,
        Right,
    }

    public record ExprBitShift(BitDirection Direction, Expression Left, Expression Right) : Expression;

    public class ExprBitShiftParser : ExpressionParser
    {
        public ExprBitShiftParser(int priority = ExpressionPriorities.BitShift) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens == null || tokens.Length < 3)
            {
                return false; // Not enough tokens to parse a bit shift expression
            }
            // find operator
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorShiftedLeftBy, out int leftOpIndex)
                || !tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorShiftedRightBy, out leftOpIndex))
            {
                return false; // No bit shift operator found
            }
            BitDirection dir = tokens[leftOpIndex].Type == TokenType.OperatorShiftedLeftBy
                ? BitDirection.Left
                : BitDirection.Right;
            // Parse left and right expressions
            if (!Expression.TryParse(tokens[..leftOpIndex], out var leftExpr)
                || !Expression.TryParse(tokens[(leftOpIndex + 1)..], out var rightExpr))
            {
                return false; // Failed to parse left or right expression
            }

            // Create the bit shift expression
            expression = new ExprBitShift(dir, leftExpr!, rightExpr!);
            return true;
        }
    }

}
