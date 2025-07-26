using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public enum ExprMathOpType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Power,
        Modulus,
    }

    public static class ExprMathOpTypeEx
    {
        public static string ToString(this ExprMathOpType type)
        {
            return type switch
            {
                ExprMathOpType.Addition => "+",
                ExprMathOpType.Subtraction => "-",
                ExprMathOpType.Multiplication => "*",
                ExprMathOpType.Division => "/",
                ExprMathOpType.Power => "^",
                ExprMathOpType.Modulus => "%",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }

    /// <summary>
    /// Contain addition multiplication, division, subtraction, power, and modulus
    /// </summary>
    public record ExprMathOp(ExprMathOpType Type, Expression Left, Expression Right) : Expression;

    public class ExprMathOpParser : ExpressionParser
    {
        public ExprMathOpParser()
            : base(ExpressionPriorities.MathOperation) { }
        public ExprMathOpParser(int priority = ExpressionPriorities.MathOperation) : base(priority) // Assuming lowest priority for math operations
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            // [] condensed tokens
            // n [raised to the power of] m <-- 3 tokens
            // n modulo m <-- 3 tokens
            // [the sum of] n [and] m <-- 4 tokens <-- the basic 4 math operators
            if (tokens.Length < 3)
                return false; // Invalid number of tokens for a math operation

            // Try to find the operator token
            int idxOperator = tokens[0].Type switch
            {
                TokenType.KeywordTheSumOf or
                TokenType.KeywordTheDifferenceOf or
                TokenType.KeywordTheProductOf or
                TokenType.KeywordTheQuotientOf => 0, // The operator is the second token
                _ => -1
            };
            bool monoTokenOperator = false;
            // check for modulo or power operator
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordModulo, out idxOperator) &&
                !tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorMathPow, out idxOperator))
            {
                monoTokenOperator = true;
                return false; // Invalid syntax, expected 'modulo' or 'raised to the power of' keyword
            }

            if (idxOperator < 0)
            {
                return false; // Operator was not found
            }

            if (monoTokenOperator)
            {
                if (!Expression.TryParse(tokens[..idxOperator], out var leftExpr) ||
                    !Expression.TryParse(tokens[(idxOperator + 1)..], out var rightExpr))
                {
                    return false; // Failed to parse left or right expression
                }
                expression = new ExprMathOp(
                    Type: tokens[idxOperator].Type switch
                    {
                        TokenType.OperatorMathPow => ExprMathOpType.Power,
                        TokenType.KeywordModulo => ExprMathOpType.Modulus,
                        _ => throw new ArgumentOutOfRangeException(nameof(tokens), "Unexpected operator token")
                    },
                    Left: leftExpr!,
                    Right: rightExpr!);
            }
            else
            {
                // search for top level and token
                if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordAnd, out int idxAnd))
                {
                    return false; // Invalid syntax, expected 'and' keyword
                }
                if (idxAnd < 2 || idxAnd >= tokens.Length - 1)
                {
                    return false; // 'and' keyword must be between the left and right expressions
                }
                // Create the left and right expressions
                if (!Expression.TryParse(tokens[1..idxAnd], out var leftExpr) ||
                    !Expression.TryParse(tokens[(idxAnd + 1)..], out var rightExpr))
                {
                    return false; // Failed to parse left or right expression
                }
                // Create the ExprMathOp based on the operator type
                expression = new ExprMathOp(
                    Type: tokens[0].Type switch
                    {
                        TokenType.KeywordTheSumOf => ExprMathOpType.Addition,
                        TokenType.KeywordTheDifferenceOf => ExprMathOpType.Subtraction,
                        TokenType.KeywordTheProductOf => ExprMathOpType.Multiplication,
                        TokenType.KeywordTheQuotientOf => ExprMathOpType.Division,
                        _ => throw new ArgumentOutOfRangeException(nameof(tokens), "Unexpected operator token")
                    },
                    Left: leftExpr!,
                    Right: rightExpr!);
            }

            return true;
        }
    }
}
