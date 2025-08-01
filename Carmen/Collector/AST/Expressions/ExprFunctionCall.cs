﻿using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public record ExprFunctionCall(Expression Identifier, Expression? Argument) : Expression;

    public class ExprFunctionCallParser : ExpressionParser
    {
        public ExprFunctionCallParser() : base(ExpressionPriorities.FunctionCall) { }
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
                if (!Expression.TryParse(tokens[1..withIndex], out var identifier) || 
                    identifier is not ExprIdentifier && identifier is not ExprMemberAccess)
                {
                    return false; // Failed to parse the identifier. Identifiers are // either a variable or a member access.
                }
                if (!Expression.TryParse(tokens[(withIndex + 1)..], out var argument))
                {
                    return false;
                }

                if (identifier is ExprIdentifier idr)
                {
                    if (idr.Type != IdentifierType.Function) // if it is an if is it a function id
                        return false;
                }

                expression = new ExprFunctionCall(identifier!, argument);
                return true;
            }
            else
            {
                if (!Expression.TryParse(tokens[1..], out var identifier) || identifier is not ExprIdentifier && identifier is not ExprMemberAccess)
                    return false;

                if (identifier is ExprIdentifier idr)
                {
                    if (idr.Type != IdentifierType.Function)
                        return false;
                }

                expression = new ExprFunctionCall(identifier!, null);
                return true;
            }
        }
    }
}