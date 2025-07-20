using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprTypeCast(Expression Expression, Expression Type, bool Safe) : Expression
    {
        public override string ToString() => $"{Expression} as a {Type}";
    }
    public class ExprTypeCastParser : ExpressionParser 
    {
        public ExprTypeCastParser(int priority = ExpressionPriorities.TypeCast) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression) // ('safe' | 'unsafe')? 'cast' OBJECT 'as a' TYPE
        {
            expression = null;
            if (tokens.Length < 4) return false; // Minimum without safe/unsafe -- use safe by default
            bool safe = true;
            bool safeExplicit = false; // safe/unsafe explicitly defined used first keyword
            if (tokens.Length >= 5)
            {
                safeExplicit = true;
                safe = tokens[0].Type == TokenType.KeywordSafe;
            }
            // Check for cast keyword
            if (!(tokens[(safeExplicit ? 1 : 0)].Type == TokenType.KeywordCast)) return false;
            // Find the 'as a' keyword
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordAsA, out var idxAsA) || idxAsA < (safeExplicit ? 3 : 2)
                || idxAsA > tokens.Length - 2) return false;
            // Parse object expression
            if (!Expression.TryParse(tokens[(safeExplicit? 2 : 1) .. idxAsA], out var objExpression)) return false;
            // Parse type expression
            if (!Expression.TryParse(tokens[(idxAsA + 1)..], out var typeExpression)) return false;
            expression = new ExprTypeCast(objExpression!, typeExpression!, safe);
            return true;
        }
    }
}
