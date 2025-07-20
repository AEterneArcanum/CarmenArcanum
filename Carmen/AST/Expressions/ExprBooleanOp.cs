using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public enum BooleanOperator
    {
        And,    // Logical AND (&&)
        Or,     // Logical OR (||)
        Xor,    // Logical XOR (^)
        //Not     // Logical NOT (!) Unary shall handle separately
    }
    public static class BooleanOperatorEx
    {
        public static string ToString(this BooleanOperator op)
        {
            return op switch
            {
                BooleanOperator.And => "&&",
                BooleanOperator.Or => "||",
                BooleanOperator.Xor => "^",
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
    }
    /// <summary>
    /// Represent boolean operations in the AST.
    /// </summary>
    public record ExprBooleanOp(BooleanOperator Operator, Expression Left, Expression Right) : Expression
    {
        public override string ToString() => $"{Left} {Operator.ToString()} {Right}";
    }
    public class ExprBooleanOpParser : ExpressionParser
    {
        public ExprBooleanOpParser(int priority = ExpressionPriorities.BooleanOperation) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            // Check if there are enough tokens to parse a boolean operation
            if (tokens.Length < 3)
            {
                return false; // Not enough tokens for a boolean operation
            }
            // Find the operator
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordAnd, out int idxOperator) &&
                !tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordOr, out idxOperator) &&
                !tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorXor, out idxOperator))
            {
                return false; // No operator found
            }
            // Get right operand
            if (!Expression.TryParse(tokens[(idxOperator + 1)..], out var rightOperand))
            {
                return false; // No right operand found
            }
            // Get left operand
            if (!Expression.TryParse(tokens[..idxOperator], out var leftOperand))
            {
                return false; // No left operand found
            }
            BooleanOperator opCode = tokens[idxOperator].Type switch
            {
                TokenType.KeywordAnd => BooleanOperator.And,
                TokenType.KeywordOr => BooleanOperator.Or,
                TokenType.OperatorXor => BooleanOperator.Xor,
                _ => throw new InvalidOperationException("Unexpected boolean operator.")
            };
            expression = new ExprBooleanOp(opCode, leftOperand!, rightOperand!);
            return true;
        }
    }
}
