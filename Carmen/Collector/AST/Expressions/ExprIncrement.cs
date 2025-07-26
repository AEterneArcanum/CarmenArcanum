using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public record ExprIncrement(Expression Identifier, bool IsPrefix) : Expression;

    public class ExprIncrementParser : ExpressionParser
    {
        public ExprIncrementParser()
            : base(ExpressionPriorities.Increment) { }
        public ExprIncrementParser(int priority = ExpressionPriorities.Increment) : base(priority) // Assuming lowest priority for increment expressions
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.OperatorIncrement && tokens[^1].Type != TokenType.OperatorIncrement) return false;
            bool isPrefix = tokens[0].Type == TokenType.OperatorIncrement;
            if (!Expression.TryParse(isPrefix ? tokens[1..] : tokens[..^1], out var identifier) 
                || identifier is not ExprIdentifier && identifier is not ExprMemberAccess && identifier is not ExprArrayAccess) // may be array access or var id
            {
                return false; // Failed to parse the identifier
            }
            expression = new ExprIncrement(identifier!, isPrefix);
            return true;
        }
    }
}
