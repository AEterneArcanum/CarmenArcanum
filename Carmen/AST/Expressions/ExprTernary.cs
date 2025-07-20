using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprTernary(Expression Condition, Expression TrueExpression, Expression FalseExpression) : Expression
    {
        public override string ToString()
        {
            return $"{Condition} ? {TrueExpression} : {FalseExpression}";
        }
    }

    public class ExprTernaryParser : ExpressionParser
    {
        public ExprTernaryParser(int priority = ExpressionPriorities.TernaryExpression) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length < 6) return false;
            if (tokens[0].Type != TokenType.KeywordIf) return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordThen, out var idxThen)) return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordOtherwise, out var idxElse)) return false;
            if (idxThen < 1 || idxElse < idxThen + 1 || idxElse >= tokens.Length - 1) return false;
            // Try Get the condition expression
            if (!Expression.TryParse(tokens[1..idxThen], out var condition)) return false;
            // Try Get the true expression
            if (!Expression.TryParse(tokens[(idxThen + 1)..idxElse], out var trueExpression)) return false;
            // Try Get the false expression
            if (!Expression.TryParse(tokens[(idxElse + 1)..], out var falseExpression)) return false;
            // Create the ternary expression
            expression = new ExprTernary(condition!, trueExpression!, falseExpression!);
            return true;
        }
    }
}
