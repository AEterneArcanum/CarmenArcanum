using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprDecrement(Expression Identifier, bool IsPrefix) : Expression;

    public class ExprDecrementParser : ExpressionParser
    {
        public ExprDecrementParser(int priority = ExpressionPriorities.Decrement) : base(priority) // Assuming lowest priority for decrement expressions
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length < 2 || (tokens[0].Type != TokenType.OperatorDecrement && tokens[^1].Type != TokenType.OperatorDecrement))
                return false;
            bool isPrefix = tokens[0].Type == TokenType.OperatorDecrement;
            if (!Expression.TryParse((isPrefix ? tokens[1..] : tokens[..^1]), out var identifier)
                || (identifier is not ExprIdentifier 
                    && identifier is not ExprMemberAccess 
                    && identifier is not ExprArrayAccess)) // may be array access or var id
            {
                return false; // Failed to parse the identifier
            }
            expression = new ExprDecrement(identifier!, isPrefix);
            return true;
        }
    }
}
