using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprFunctionCall(Expression Identifier, Expression? Argument) : Expression
    {
        public override string ToString()
        {
            return Argument is null ? $"{Identifier}" : $"{Identifier}({Argument})";
        }
    }

    public class ExprFunctionCallParser : ExpressionParser
    {
        public ExprFunctionCallParser(int priority = ExpressionPriorities.FunctionCall) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.KeywordCall)
            {
                return false;
            }
            bool hasArguments = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordWith, out int withIndex);
            if (hasArguments)
            {
                if (!Expression.TryParse(tokens[1..withIndex], out var identifier) || (identifier is not ExprIdentifier && identifier is not ExprMemberAccess))
                {
                    return false; // Failed to parse the identifier. Identifiers are // either a variable or a member access.
                }
                if (!Expression.TryParse(tokens[(withIndex + 1)..], out var argument))
                {
                    return false;
                }
                expression = new ExprFunctionCall(identifier!, argument);
                return true;
            }
            else
            {
                if (!Expression.TryParse(tokens[1..], out var identifier) || (identifier is not ExprIdentifier && identifier is not ExprMemberAccess))
                {
                    return false;
                }
                expression = new ExprFunctionCall(identifier!, null);
                return true;
            }
        }
    }
}