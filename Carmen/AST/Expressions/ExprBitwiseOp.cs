using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public enum BitwiseOperation
    {
        And,
        Or,
        Xor,
        //Not, All Handled elsewhere
        //ShiftLeft,
        //ShiftRight,
        //RotateLeft,
        //RotateRight
    }

    public record ExprBitwiseOp(BitwiseOperation Operation, Expression Left, Expression Right) : Expression;

    public class ExprBitwiseOpParser : ExpressionParser
    {
        public ExprBitwiseOpParser(int priority = ExpressionPriorities.BitwiseOperation) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens == null || tokens.Length < 3)
            {
                return false; // Not enough tokens to form a bitwise operation
            }
            // Find the operator
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorBitwiseAnd, out int idxOperator) &&
                !tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorBitwiseOr, out idxOperator) &&
                !tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorBitwiseXor, out idxOperator))
            {
                return false; // No bitwise operator found
            }
            // Get left operand
            if (!Expression.TryParse(tokens[..idxOperator], out var leftExpr))
            {
                return false; // No left operand found
            }
            // Get right operand
            if (!Expression.TryParse(tokens[(idxOperator + 1)..], out var rightExpr))
            {
                return false; // No right operand found
            }
            BitwiseOperation operation = tokens[idxOperator].Type switch
            {
                TokenType.OperatorBitwiseAnd => BitwiseOperation.And,
                TokenType.OperatorBitwiseOr => BitwiseOperation.Or,
                TokenType.OperatorBitwiseXor => BitwiseOperation.Xor,
                _ => throw new InvalidOperationException("Unexpected bitwise operator.")
            };
            expression = new ExprBitwiseOp(operation, leftExpr!, rightExpr!);
            return true;
        }
    }
}
