namespace Arcane.Script.Carmen;

public static partial class Parser
{
    public static Statement[] Parse(Token[] tokens)
    {
        List<Token[]> separatedTokens = _getSeparatedTokens(tokens); // Get tokens separated by EOL symbol
                                                                     // Each index of separated tokens is now one complete script command.
        return _getParsedStatements(separatedTokens).ToArray();
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
