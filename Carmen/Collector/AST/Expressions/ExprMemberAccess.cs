using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Owner">Owning object</param>
    /// <param name="Member">Accessed member</param>
    public record ExprMemberAccess(ExprIdentifier Owner, Expression Member) : Expression;

    public class ExprMemberAccessParser : ExpressionParser
    {
        public ExprMemberAccessParser()
            : base(ExpressionPriorities.MemberAccess) { }
        public ExprMemberAccessParser(int priority = ExpressionPriorities.MemberAccess) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            // Valid patterns:
            // $object 's $variable
            // Check if there are at least 3 tokens
            if (tokens.Length < 3)
            {
                expression = null; // Not enough tokens to form a member access expression
                return false;
            }
            // look for the first top layer index of the "in" keyword
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.OperatorMemberAccess, out int idxIn))
            {
                expression = null; // "in" keyword not found
                return false;
            }
            // try parse member expression must be a struct or enum or future structures < -- all will use same id structure
            if (!Expression.TryParse(tokens[..idxIn], out var owner) ||
                owner is not ExprIdentifier owID || !owID.Type.IsContainerType())
            {
                expression = null; // Failed to parse the member expression
                return false;
            }
            // try parse accessed expression
            if (!Expression.TryParse(tokens[(idxIn + 1)..], out var member)) // member can be further member access property functions
            {
                expression = null; // Failed to parse the accessed expression
                return false;
            }
            expression = new ExprMemberAccess(owID!, member!);
            return true;
        }
    }
}
