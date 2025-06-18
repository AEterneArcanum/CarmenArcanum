namespace Arcane.Script.Carmen;

public static class Parser
{
    public static Statement[] Parse(Token[] tokens)
    {
        List<Token[]> separatedTokens = _getSeparatedTokens(tokens); // Get tokens separated by EOL symbol
                                                                     // Each index of separated tokens is now one complete script command.
        return _getParsedStatements(separatedTokens).ToArray();
    }

    private static List<Statement> _getParsedStatements(List<Token[]> tokens)
    {
        List<Statement> statements = [];
        foreach (var s in tokens)
            statements.Add(_getParsedStatement(s));
        return statements;
    }

    private static Statement _getParsedStatement(Token[] tokens)
    {
        // Put Statement
        if (tokens.Length >= 5 // Minimum 5 symbols in put statement.
            && tokens[0].Type == TokenType.PUT
            && tokens[^3].Type == TokenType.INTO
            && tokens[^2].Type == TokenType.IDENTIFIER
            && tokens[^1].Type == TokenType.EOL)
        {
            // Maybe validate variable type here somehow??
            return new PutIntoStatement(
                new Identifier(tokens[^2].Value),
                _getParsedExpression(tokens[1..^3])
            );
        }
        // Variable Definition
        if (tokens.Length >= 5 // Minimum 5 symbols in variable definition.
            && tokens[0].Type == TokenType.IDENTIFIER
            && tokens[1].Type == TokenType.IS
            && tokens[2].Type == TokenType.A
            && tokens[^1].Type == TokenType.EOL)
        {
            // VarType
            VariableType t = tokens[3].Type switch
            {
                TokenType.INTEGER => VariableType.INTEGER,
                TokenType.STRING => VariableType.STRING,
                TokenType.BOOL => VariableType.BOOL,
                _ => throw new InvalidProgramException($"Invalid Variable Type: {tokens[3].Type}")
            };
            // TODO: Add support initial value
            Expression initial = t switch
            {
                VariableType.INTEGER => new IntegerLiteral(0),
                VariableType.STRING => new StringLiteral(string.Empty),
                VariableType.BOOL => new BooleanLiteral(true),
                _ => throw new InvalidProgramException($"No Default Initial For Variable Type: {tokens[3].Type}")
            };

            return new VariableDefinitionStatement(
                new Identifier(tokens[0].Value),
                t, initial
            );
        }
        // Check For Label Statement
        if (tokens.Length == 3 // Exactly 3 symbols in a label statement.
            && tokens[0].Type == TokenType.LABEL
            && tokens[1].Type == TokenType.IDENTIFIER
            && tokens[2].Type == TokenType.EOL)
        {
            return new LabelStatement(new Identifier(tokens[1].Value));
        }
        // Check For GoTo Statement
        if (tokens.Length == 3 // Exactly 3 symbols in a goto statement
            && tokens[0].Type == TokenType.GOTO
            && tokens[1].Type == TokenType.IDENTIFIER
            && tokens[2].Type == TokenType.EOL)
        {
            return new GoToStatement(new Identifier(tokens[1].Value));
        }
        // Display statement
        if (tokens.Length >= 3 // Minimum 3 symbols in display statement.
            && tokens[0].Type == TokenType.DISPLAY
            && tokens[^1].Type == TokenType.EOL)
        {
            Expression val = _getParsedExpression(tokens[1..^1]);
            return new DisplayStatement(val);
        }
        // Increment statement
        if (tokens.Length >= 3
            && tokens[0].Type == TokenType.INCREMENT
            && tokens[^1].Type == TokenType.EOL) // increment x. min 3 symbols
        {
            Expression val = _getParsedExpression(tokens[1..^1]);
            if (val is not Identifier)
                throw new NotImplementedException("Only var names suppoted in increment statement");
            return new IncrementStatement((Identifier)val);
        }
        // if statement ... if x then y. 5 minimum
        if (tokens.Length >= 5
            && tokens[0].Type == TokenType.IF
            && tokens[^1].Type == TokenType.EOL
            && tokens.Any(x => x.Type == TokenType.THEN))
        {
            int indexOfThen = Array.IndexOf(tokens, tokens.First(x => x.Type == TokenType.THEN));
            Expression expr = _getParsedExpression(tokens[1..indexOfThen]);
            Statement stmt = _getParsedStatement(tokens[(indexOfThen + 1)..]);
            return new IfStatement(expr, stmt);
        }
        throw new NotImplementedException("Statement not implemented.");
    }

    private static Expression _getParsedExpression(Token[] tokens)
    {
        if (tokens.Length == 1)
        {
            if (tokens[0].Type == TokenType.LITERAL_STRING)
                return new StringLiteral(
                    tokens[0]
                        .Value
                        .TrimStart(Symbols.LITERAL_STRING_START[0])
                        .TrimEnd(Symbols.LITERAL_STRING_END[0]));
            if (tokens[0].Type == TokenType.LITERAL_INTEGER)
                return new IntegerLiteral(int.Parse(tokens[0].Value));
            if (tokens[0].Type == TokenType.IDENTIFIER)
                return new Identifier(tokens[0].Value);
            if (tokens[0].Type == TokenType.TRUE)
                return new BooleanLiteral(true);
            if (tokens[0].Type == TokenType.FALSE)
                return new BooleanLiteral(false);
        }
        if (tokens.Length == 2)
        {
            if (tokens[0].Type == TokenType.THE
                && tokens[1].Type == TokenType.RECEIVED)
            {
                return new ReceiveInputExpression();
            }
        }
        if (tokens.Length >= 4) // x, (and|or) y
        {
            // Find each instance of [, and] [, or]
            // Existence of comma
            if (tokens.Any(x => x.Type == TokenType.COMMA))// this structure will not support ands/ors within ands/ors without parenthesis
            {
                List<int> commaIndices = []; // All Comma Index Positions
                for (int i = 1; i < tokens.Length - 1; i++)
                {
                    if (tokens[i].Type == TokenType.COMMA) commaIndices.Add(i);
                }
                // Check for or statements
                for (int i = 0; i < commaIndices.Count; i++)
                {
                    int commaIndex = commaIndices[i];
                    if (tokens[commaIndex + 1].Type == TokenType.OR)
                    {
                        // recurse first Or Statement
                        var parta = _getParsedExpression(tokens[..commaIndex]);
                        var partb = _getParsedExpression(tokens[(commaIndex + 2)..]);
                        return new BinaryExpression(BinaryOperationType.OR, parta, partb);
                    }
                }
                // No More Or Statements look for and statements
                for (int i = 0; i < commaIndices.Count; i++)
                {
                    int commaIndex = commaIndices[i];
                    if (tokens[commaIndex + 1].Type == TokenType.AND)
                    {
                        var parta = _getParsedExpression(tokens[..commaIndex]);
                        var partb = _getParsedExpression(tokens[(commaIndex + 2)..]);
                        return new BinaryExpression(BinaryOperationType.AND, parta, partb);
                    }
                }
                // No And Or Or but comma exists ?? was the comma necessary -- Malformed expression
                throw new InvalidProgramException("Malformed Expression.");
            }
            if (tokens.Any(x => x.Type == TokenType.IS))
            {
                if (tokens.Length >= 4)
                {
                    // try to find is a expression
                    int indexOfIs = Array.IndexOf(tokens, tokens.First(x => x.Type == TokenType.IS));
                    if (tokens[indexOfIs + 1].Type == TokenType.A)
                    {
                        var parta = _getParsedExpression(tokens[..indexOfIs]);
                        var partb = _getParsedExpression(tokens[(indexOfIs + 2)..]);
                        return new BinaryExpression(BinaryOperationType.ISTYPE, parta, partb);
                    }
                }

                // These Are nested to they dont trigger on sub parts already processed above
                if (tokens.Length >= 5) // x (is equal to | is less than | is greater than) y minimum 5 for equality
                                        // only support single symbol this time v2 add parenthesis and change comment symbols
                                        // I dont have a method for linguisticaly separating complex expressions 
                                        // other than complicated comma placement eg. EXPRESSION[, and] EXPRESSION[, or] EXPRESSION
                                        // here the [, and/or] is a complex token that will need to be found / existence checked
                                        // processed via Another Layer of Binary expression [or]->[and][and] << each and 
                                        // on either side of an or
                {
                    // find phrase existence and position
                    // recursively get x and y
                    // return correct binary expression

                    // If 'is' is immediately followed by 'a' it is an is type expression

                    bool not = tokens.Any(x => x.Type == TokenType.NOT);
                    int indexOfIs = Array.IndexOf(tokens, tokens.First(x => x.Type == TokenType.IS));
                    int indexOfMode = indexOfIs + ((not) ? 2 : 1);
                    if (tokens[indexOfMode].Type == TokenType.EQUAL
                        && tokens[indexOfMode + 1].Type == TokenType.TO)
                    {
                        var pa = _getParsedExpression(tokens[..indexOfIs]);
                        var pb = _getParsedExpression(tokens[(indexOfMode + 2)..]);
                        return new BinaryExpression((not) ? BinaryOperationType.NOTEQUALTO : BinaryOperationType.EQUALTO, pa, pb);
                    }
                    if (tokens[indexOfMode].Type == TokenType.LESS
                        && tokens[indexOfMode + 1].Type == TokenType.THAN)
                    {
                        var pa = _getParsedExpression(tokens[..indexOfIs]);
                        var pb = _getParsedExpression(tokens[(indexOfMode + 2)..]);
                        return new BinaryExpression((not) ? BinaryOperationType.NOTLESSTHAN : BinaryOperationType.LESSTHAN, pa, pb);
                    }
                    if (tokens[indexOfMode].Type == TokenType.GREATER
                        && tokens[indexOfMode + 1].Type == TokenType.THAN)
                    {
                        var pa = _getParsedExpression(tokens[..indexOfIs]);
                        var pb = _getParsedExpression(tokens[(indexOfMode + 2)..]);
                        return new BinaryExpression((not) ? BinaryOperationType.NOTGREATERTHAN : BinaryOperationType.GREATERTHAN, pa, pb);
                    }
                    // Is Exists but malformed
                    throw new InvalidProgramException("Malformed Is Expressions");
                }
            }
            if (tokens.Length >= 6) // the (sum|difference|product|quotient|modulus) of x and y // minimum 6 for math
            // only support single symbol this time v2 add ...
            {
                // find phrase existence and kind additionally position of 'and' symbol
                // recursivly get x and y
                // return correct binary expression
                if (tokens[0].Type == TokenType.THE
                    && tokens[2].Type == TokenType.OF
                    && tokens.Any(x => x.Type == TokenType.AND))
                {
                    int indexOfAnd = Array.IndexOf(tokens, tokens.First(x => x.Type == TokenType.AND));
                    var parta = _getParsedExpression(tokens[3..indexOfAnd]);
                    var partb = _getParsedExpression(tokens[(indexOfAnd + 1)..]);
                    return tokens[1].Type switch
                    {
                        TokenType.SUM => new BinaryExpression(BinaryOperationType.ADDITION, parta, partb),
                        TokenType.DIFFERENCE => new BinaryExpression(BinaryOperationType.SUBTRACTION, parta, partb),
                        TokenType.PRODUCT => new BinaryExpression(BinaryOperationType.MULTIPLICATION, parta, partb),
                        TokenType.QUOTIENT => new BinaryExpression(BinaryOperationType.DIVISION, parta, partb),
                        TokenType.MODULUS => new BinaryExpression(BinaryOperationType.MODULUS, parta, partb),
                        _ => throw new InvalidProgramException("Unrecognized operation expression.")
                    };
                }
            }
        }

        throw new NotImplementedException("Expression not implemented.");
    }

    private static List<Token[]> _getSeparatedTokens(Token[] tokens)
    {
        List<Token[]> sepTok = [];
        List<Token> crntCom = [];

        for (int i = 0; i < tokens.Length; i++)
        {
            crntCom.Add(tokens[i]);
            if (tokens[i].Type == TokenType.EOL)
            {
                sepTok.Add(crntCom.ToArray());
                crntCom = [];
            }
        }

        return sepTok;
    }
}
