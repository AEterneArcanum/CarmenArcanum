namespace Arcane.Script.Carmen;

public static partial class Parser
{
    private static List<Statement> _getParsedStatements(List<Token[]> tokens)
    {
        List<Statement> statements = [];
        foreach (var s in tokens)
            statements.Add(_getParsedStatement(s));
        return statements;
    }

    private static Statement _getParsedStatement(Token[] tokens)
    {
        if (_tryParsePutInto(tokens, out var statement))
            return statement ?? throw new InvalidProgramException("How did we get here?");
        if (_tryParseVarDef(tokens, out statement))
            return statement ?? throw new InvalidProgramException("How did we get here?");
        if (_tryParseLabel(tokens, out statement))
            return statement ?? throw new InvalidProgramException("How did we get here?");
        if (_tryParseGoTo(tokens, out statement))
            return statement ?? throw new InvalidProgramException("How did we get here?");
        if (_tryParseIncrement(tokens, out statement))
            return statement ?? throw new InvalidProgramException("How did we get here?");
        if (_tryParseDisplay(tokens, out statement))
            return statement ?? throw new InvalidProgramException("How did we get here?");
        if (_tryParseIfThen(tokens, out statement))
            return statement ?? throw new InvalidProgramException("How did we get here?");
        throw new NotImplementedException("Statement not implemented.");
    }

    private static bool _tryParseIfThen(Token[] tokens, out IfStatement? statement)
    {
        statement = null;
        if (tokens.Length < 5
            || tokens[0].Type != TokenType.IF
            || tokens[^1].Type != TokenType.EOL
            || !tokens.Any(x => x.Type == TokenType.THEN))
            return false;
        int indexOfThen = Array.IndexOf(tokens, tokens.First(x => x.Type == TokenType.THEN));
        statement = new IfStatement(
            _getParsedExpression(tokens[1..indexOfThen]),
            _getParsedStatement(tokens[(indexOfThen + 1)..])
        );
        return true;
    }

    private static bool _tryParseDisplay(Token[] tokens, out DisplayStatement? statement)
    {
        statement = null;
        if (tokens.Length < 3
            || tokens[0].Type != TokenType.DISPLAY
            || tokens[^1].Type != TokenType.EOL)
            return false;
        statement = new DisplayStatement(_getParsedExpression(tokens[1..^1]));
        return true;
    }

    private static bool _tryParseIncrement(Token[] tokens, out IncrementStatement? statement)
    {
        statement = null;
        if (tokens.Length != 3
            || tokens[0].Type != TokenType.INCREMENT
            || tokens[1].Type != TokenType.IDENTIFIER
            || tokens[2].Type != TokenType.EOL)
            return false;
        statement = new IncrementStatement(new Identifier(tokens[1].Value));
        return true;
    }

    private static bool _tryParseGoTo(Token[] tokens, out GoToStatement? statement)
    {
        statement = null;
        if (tokens.Length != 3
            || tokens[0].Type != TokenType.GOTO
            || tokens[1].Type != TokenType.IDENTIFIER
            || tokens[2].Type != TokenType.EOL)
            return false;
        statement = new GoToStatement(new Identifier(tokens[1].Value));
        return true;
    }

    private static bool _tryParseLabel(Token[] tokens, out LabelStatement? statement)
    {
        statement = null;
        if (tokens.Length != 3
            || tokens[0].Type != TokenType.LABEL
            || tokens[1].Type != TokenType.IDENTIFIER
            || tokens[2].Type != TokenType.EOL)
            return false;
        statement = new LabelStatement(new Identifier(tokens[1].Value));
        return true;
    }

    private static bool _tryParseVarDef(Token[] tokens, out VariableDefinitionStatement? statement)
    {
        statement = null;
        if (tokens.Length != 5
            || tokens[0].Type != TokenType.IDENTIFIER
            || tokens[1].Type != TokenType.IS
            || tokens[2].Type != TokenType.A
            || tokens[4].Type != TokenType.EOL)
            return false;
        VariableType t = tokens[3].Type switch
        {
            TokenType.INTEGER => VariableType.INTEGER,
            TokenType.STRING => VariableType.STRING,
            TokenType.BOOL => VariableType.BOOL,
            _ => throw new InvalidProgramException($"Invalid Variable Type: {tokens[3].Type}")
        };
        Expression initial = t switch
        {
            VariableType.INTEGER => new IntegerLiteral(0),
            VariableType.STRING => new StringLiteral(string.Empty),
            VariableType.BOOL => new BooleanLiteral(true),
            _ => throw new InvalidProgramException($"No Default Initial For Variable Type: {tokens[3].Type}")
        };
        statement = new VariableDefinitionStatement(
            new Identifier(tokens[0].Value),
            t, initial);
        return true;
    }

    private static bool _tryParsePutInto(Token[] tokens, out PutIntoStatement? statement)
    {
        statement = null;
        if (tokens.Length < 5
            || tokens[0].Type != TokenType.PUT
            || tokens[^3].Type != TokenType.INTO
            || tokens[^2].Type != TokenType.IDENTIFIER
            || tokens[^1].Type != TokenType.EOL)
            return false;
        statement = new PutIntoStatement(
            new Identifier(tokens[^2].Value),
            _getParsedExpression(tokens[1..^3]));
        return true;
    }
}