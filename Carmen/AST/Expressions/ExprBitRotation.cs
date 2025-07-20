using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprBitRotation(BitDirection Direction, Expression Left, Expression Right)
        : Expression
    {
        public override string ToString() =>
            $"{Left} {((Direction == BitDirection.Right) ? " ROR " : " ROL ")} {Right}";
    }

    public class ExprBitRotationParser : ExpressionParser
    {
        public ExprBitRotationParser(int priority = ExpressionPriorities.BitShift) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens == null || tokens.Length < 3)
            {
                return false; // Not enough tokens to parse a bit rotation expression
            }
            // find operator
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorRotatedLeftBy, out int leftOpIndex)
                || !tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorRotatedRightBy, out leftOpIndex))
            {
                return false; // No bit rotation operator found
            }
            BitDirection dir = tokens[leftOpIndex].Type == TokenType.OperatorRotatedLeftBy
                ? BitDirection.Left
                : BitDirection.Right;
            // Parse left and right expressions
            if (!Expression.TryParse(tokens[..leftOpIndex], out var leftExpr)
                || !Expression.TryParse(tokens[(leftOpIndex + 1)..], out var rightExpr))
            {
                return false; // Failed to parse left or right expression
            }
            // Create the bit rotation expression
            expression = new ExprBitRotation(dir, leftExpr!, rightExpr!);
            return true;
        }
    }
}
