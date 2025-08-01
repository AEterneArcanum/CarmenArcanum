﻿using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public enum ComparisonOperator
    {
        Equal,          // ==
        NotEqual,       // !=
        GreaterThan,    // >
        LessThan,       // <
        NotGreaterThan, // >=
        NotLessThan,    // <=
        //And,            // && (Logical AND)
        //Or,             // || (Logical OR)
        //Xor,            // ^ (Logical XOR)
        //Not             // ! (Logical NOT) // Only single operand, not a binary operator
    }

    public static class ComparisonOperatorEx
    {
        public static string OperatorToString(this ComparisonOperator op)
        {
            return op switch
            {
                ComparisonOperator.Equal => "==",
                ComparisonOperator.NotEqual => "!=",
                ComparisonOperator.GreaterThan => ">",
                ComparisonOperator.LessThan => "<",
                ComparisonOperator.NotGreaterThan => ">=",
                ComparisonOperator.NotLessThan => "<=",
                _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
            };
        }
    }

    public record ExprComparisonOp(ComparisonOperator Operator, Expression Left, Expression Right) : Expression;

    public class ExprComparisonOpParser : ExpressionParser
    {
        public ExprComparisonOpParser()
            : base(ExpressionPriorities.Comparison) { }
        public ExprComparisonOpParser(int priority = ExpressionPriorities.Comparison) : base(priority) // Assuming lowest priority for comparison operators
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            bool isInverted = false;
            // try find operator
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordEqualTo, out int idxOperator) ||
                !tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorLessThan, out idxOperator) ||
                !tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorGreaterThan, out idxOperator) ||
                !tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorLessThanOrEqualTo, out idxOperator) ||
                !tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorGreaterThanOrEqualTo, out idxOperator))
            {
                return false; // No operator found
            }
            // get right operand
            if (!Expression.TryParse(tokens[(idxOperator + 1)..], out var rightOperand))
            {
                return false; // No right operand found
            }
            // check for inversion
            if (tokens[idxOperator - 1].Type == TokenType.KeywordNot)
            {
                isInverted = true;
                idxOperator--;
            }
            // get left operand
            if (!Expression.TryParse(tokens[..idxOperator], out var leftOperand))
            {
                return false; // No left operand found
            }
            ComparisonOperator opCode = ComparisonOperator.Equal;
            if (isInverted)
            {
                // Invert the operator
                opCode = tokens[idxOperator + 1].Type switch
                {
                    TokenType.KeywordEqualTo => ComparisonOperator.NotEqual,
                    TokenType.OperatorLessThan => ComparisonOperator.NotLessThan,
                    TokenType.OperatorGreaterThan => ComparisonOperator.NotGreaterThan,
                    TokenType.OperatorLessThanOrEqualTo => ComparisonOperator.GreaterThan,
                    TokenType.OperatorGreaterThanOrEqualTo => ComparisonOperator.LessThan,
                    _ => throw new ArgumentOutOfRangeException(nameof(tokens), "Invalid operator token")
                };
            }
            else
            {
                // Use the operator as is
                opCode = tokens[idxOperator].Type switch
                {
                    TokenType.KeywordEqualTo => ComparisonOperator.Equal,
                    TokenType.OperatorLessThan => ComparisonOperator.LessThan,
                    TokenType.OperatorGreaterThan => ComparisonOperator.GreaterThan,
                    TokenType.OperatorLessThanOrEqualTo => ComparisonOperator.NotGreaterThan,
                    TokenType.OperatorGreaterThanOrEqualTo => ComparisonOperator.NotLessThan,
                    _ => throw new ArgumentOutOfRangeException(nameof(tokens), "Invalid operator token")
                };
            }

            // Create the expression
            expression = new ExprComparisonOp(opCode, leftOperand!, rightOperand!);

            return true;
        }
    }
}
