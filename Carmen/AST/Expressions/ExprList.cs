using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Expressions
{
    /// <summary>
    /// Represents a list in code "; value, value, value, and value".
    /// This code will not support lists within lists.
    /// </summary>
    public record ExprList(Expression[] Expressions) : Expression
    {
        public override string ToString()
        {
            return $"{string.Join(", ", Expressions.Select(e => e.ToString()))}";
        }
    }

    public class ExprListParser : ExpressionParser
    {
        public ExprListParser(int priority = ExpressionPriorities.ListExpression) : base(priority) // Assuming lowest priority for lists
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.PunctuationSemicolon)
                return false;
            List<Expression> expressions = new List<Expression>();

            List<int> commas = tokens.TopLayerIndicesOfXBeforeY(TokenType.PunctuationComma, TokenType.KeywordCommaAnd);
            commas.Add(tokens.FirstTopLayerIndexOf(TokenType.KeywordCommaAnd)); // Add the index of the ", and" keyword if it exists.

            // Number of commas + 1 is the number of expressions.
            int expressionCount = commas.Count + 1;
            //if (expressionCount < 1) // Logically unreachable
            //    return false;
            if (expressionCount == 1)
            {
                var expressionTokens = tokens[1..]; // All remaining token eos should not make it here.
                if (!Expression.TryParse(expressionTokens, out var contentexpression)) return false; // Failed to parse the single expression.
                expressions.Add(contentexpression!);
            }
            else
            {
                // Multiple expressions. // from index 1 to index of first comma. --> f comma to s comma --> final comma to end iteration.
                int startIndex = 1; // Start after the semicolon.
                for (int i = 0; i < commas.Count; i++)
                {
                    int endIndex = commas[i];
                    var expressionTokens = tokens[startIndex..endIndex]; // Get tokens from start to the current comma.
                    if (!Expression.TryParse(expressionTokens, out var contentexpression)) return false; // Failed to parse an expression.
                    expressions.Add(contentexpression!);
                    startIndex = endIndex + 1; // Move to the next token after the comma.
                }
                // Handle the last expression after the last comma.
                var lastExpressionTokens = tokens[startIndex..]; // Get tokens from the last comma to the end.
                if (!Expression.TryParse(lastExpressionTokens, out var lastExpression)) return false; // Failed to parse the last expression.
                expressions.Add(lastExpression!);
            }

            expression = new ExprList(expressions.ToArray());
            return true;
        }
    }
}
