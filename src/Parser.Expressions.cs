namespace Arcane.Script.Carmen;

public static partial class Parser
{
    private static Expression _getParsedExpression(Token[] tokens)
    {
        if (tokens.Length == 1 && _tryParseMonoTokenExpression(tokens[0], out var expression))
            return expression;
        if (tokens.Length == 2 && _tryParseDualTokenExpression(tokens[0], tokens[1], out expression))
            return expression;
        if (_tryParseUnionExpression(tokens, out expression))
            return expression;
        if (_tryParseComparisonExpression(tokens, out expression))
            return expression;
        if (_tryParseMathOperation(tokens, out expression))
            return expression;
        throw new NotImplementedException("Expression not implemented.");
    }
    /// <summary>
    /// Try to parse the basic 5 math operators.
    /// </summary>
    /// <param name="tokens">Available tokens.</param>
    /// <param name="expression">The parsed comparison.</param>
    /// <returns>True on parse success.</returns>
    private static bool _tryParseMathOperation(Token[] tokens, out Expression expression)
    {
        expression = new StringLiteral("ERROR: Invalid math operation.");
        if (tokens.Length < 6)
            return false;
        if (tokens[0].Type != TokenType.THE
            || tokens[2].Type != TokenType.OF
            || !tokens.Any(x => x.Type == TokenType.AND))
            return false;
        int indexOfAnd = Array.IndexOf(tokens, tokens.First(x => x.Type == TokenType.AND));
        var left = _getParsedExpression(tokens[3..indexOfAnd]);
        var right = _getParsedExpression(tokens[(indexOfAnd + 1)..]);
        expression = tokens[1].Type switch
        {
            TokenType.SUM => new BinaryExpression(BinaryOperationType.ADDITION, left, right),
            TokenType.DIFFERENCE => new BinaryExpression(BinaryOperationType.SUBTRACTION, left, right),
            TokenType.PRODUCT => new BinaryExpression(BinaryOperationType.MULTIPLICATION, left, right),
            TokenType.QUOTIENT => new BinaryExpression(BinaryOperationType.DIVISION, left, right),
            TokenType.MODULUS => new BinaryExpression(BinaryOperationType.MODULUS, left, right),
            _ => expression
        };
        if (expression is StringLiteral) return false;
        return true;
    }
    /// <summary>
    /// Try to parse equal to, greater, lesser and the inverse of each.
    /// </summary>
    /// <param name="tokens">Available tokens.</param>
    /// <param name="expression">The parsed comparison.</param>
    /// <returns>True on parse success.</returns>
    private static bool _tryParseComparisonExpression(Token[] tokens, out Expression expression)
    {
        expression = new StringLiteral("Bad comparison expression.");
        // Check for the [is] keyword
        if (!tokens.Any(x => x.Type == TokenType.IS))
            return false;
        // Find the is
        int IndexOfIs = Array.IndexOf(tokens, tokens.First(x => x.Type == TokenType.IS));
        // Check if is inverse
        bool inverse = tokens[IndexOfIs + 1].Type == TokenType.NOT;
        // If long enough for equality
        if (tokens.Length < (5 + (inverse ? 1 : 0)))
            return false;
        // check equal to
        int indexOfRight = -1; // initial index of the right side.
        BinaryOperationType type = (BinaryOperationType)100;
        if (tokens[IndexOfIs + (inverse ? 2 : 1)].Type == TokenType.EQUAL
            && tokens[IndexOfIs + (inverse ? 3 : 2)].Type == TokenType.TO)
        {
            type = inverse ? BinaryOperationType.NOTEQUALTO : BinaryOperationType.EQUALTO;
            indexOfRight = IndexOfIs + (inverse ? 4 : 3);
        }
        else if (tokens[IndexOfIs + (inverse ? 2 : 1)].Type == TokenType.THE
            && tokens[IndexOfIs + (inverse ? 3 : 2)].Type == TokenType.LESSER
            && tokens[IndexOfIs + (inverse ? 4 : 3)].Type == TokenType.OF)
        {
            type = inverse ? BinaryOperationType.NOTLESSTHAN : BinaryOperationType.LESSTHAN;
            indexOfRight = IndexOfIs + (inverse ? 5 : 4);
        }
        else if (tokens[IndexOfIs + (inverse ? 2 : 1)].Type == TokenType.THE
            && tokens[IndexOfIs + (inverse ? 3 : 2)].Type == TokenType.GREATER
            && tokens[IndexOfIs + (inverse ? 4 : 3)].Type == TokenType.OF)
        {
            type = inverse ? BinaryOperationType.NOTGREATERTHAN : BinaryOperationType.GREATERTHAN;
            indexOfRight = IndexOfIs + (inverse ? 5 : 4);
        }
        // Verify op type
        if (type == (BinaryOperationType)100)
            return false;
        // Get Left & Right
        var left = _getParsedExpression(tokens[..IndexOfIs]);
        var right = _getParsedExpression(tokens[indexOfRight..]);
        expression = new BinaryExpression(type, left, right);
        return true;
    }
    /// <summary>
    /// Try to parse and or expressions.
    /// </summary>
    /// <param name="tokens">Available tokens.</param>
    /// <param name="expression">The parsed union.</param>
    /// <returns>True on parse success.</returns>
    private static bool _tryParseUnionExpression(Token[] tokens, out Expression expression)
    {
        // Check if there are enough tokens for comparisons
        if (tokens.Length < 5)
        {
            expression = new StringLiteral("ERROR: Not enough tokens.");
            return false;
        }
        // Switch on initial token [either] || [both] invalid if otherwise
        BinaryOperationType type = tokens[0].Type switch
        {
            TokenType.EITHER => BinaryOperationType.OR,
            TokenType.BOTH => BinaryOperationType.AND,
            _ => (BinaryOperationType)100
        };
        // Check valid operation
        if (type == (BinaryOperationType)100)
        {
            expression = new StringLiteral("ERROR: Not valid union expression.");
            return false;
        }
        // Look for index of first comma
        if (!tokens.Any(x => x.Type == TokenType.COMMA))
        {
            expression = new StringLiteral("ERROR: No commas");
            return false;
        }
        int commaIndex = Array.IndexOf(tokens, tokens.First(x => x.Type == TokenType.COMMA));
        // check for proper and/or
        if (type == BinaryOperationType.AND)
        {
            if (tokens[commaIndex + 1].Type != TokenType.AND)
            {
                expression = new StringLiteral("ERROR: No 'and' in and comparison.");
                return false;
            }
            else if (tokens[commaIndex + 1].Type != TokenType.OR)
            {
                expression = new StringLiteral("ERROR: No 'or' in or comparison.");
                return false;
            }
        }
        // parse side expressions
        var left = _getParsedExpression(tokens[1..commaIndex]);
        var right = _getParsedExpression(tokens[(commaIndex + 2)..]);
        expression = new BinaryExpression(type, left, right);
        return true;
    }
    /// <summary>
    /// Try to parse an expression that always has two tokens.
    /// </summary>
    /// <param name="a">The first token.</param>
    /// <param name="b">The second token.</param>
    /// <param name="expression">The parsed expression.</param>
    /// <returns>True on parse success.</returns>
    private static bool _tryParseDualTokenExpression(Token a, Token b, out Expression expression)
    {
        if (a.Type == TokenType.THE
            && b.Type == TokenType.RECEIVED)
        {
            expression = new ReceiveInputExpression();
            return true;
        }
        expression = new StringLiteral($"ERROR: Dual token expression not parsed. {a} {b}");
        return false;
    }
    /// <summary>
    /// Try to parse a single token expression.
    /// </summary>
    /// <param name="token">Token to parse.</param>
    /// <param name="expression">The parsed expression.</param>
    /// <returns>True on parse success.</returns>
    private static bool _tryParseMonoTokenExpression(Token token, out Expression expression)
    {
        switch (token.Type)
        {
            case TokenType.LITERAL_STRING:
                expression = new StringLiteral(token
                    .Value
                    .TrimStart(Symbols.LITERAL_STRING_START[0])
                    .TrimEnd(Symbols.LITERAL_STRING_END[0]));
                return true;
            case TokenType.LITERAL_INTEGER:
                expression = new IntegerLiteral(int.Parse(token.Value));
                return true;
            case TokenType.IDENTIFIER:
                expression = new Identifier(token.Value);
                return true;
            case TokenType.TRUE:
                expression = new BooleanLiteral(true);
                return true;
            case TokenType.FALSE:
                expression = new BooleanLiteral(false);
                return true;
            default:
                expression = new StringLiteral($"ERROR: Mono Token expression not parsed. {token}");
                return false;
        }
    }
}