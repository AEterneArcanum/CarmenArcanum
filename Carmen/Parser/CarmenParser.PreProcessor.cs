
using Arcane.Carmen.Lexer;

namespace Arcane.Carmen.Parser;

public partial class CarmenParser
{
    /// <summary>
    /// Remove comment and whitespace tokens.
    /// Find complex token chains and combine into single tokens.
    /// (ex. 'is' 'less' 'than' -> 'is' 'less than')
    /// </summary>
    /// <param name="tokens">Tokens to process.</param>
    /// <returns>Processed tokens.</returns>
    private List<Token> Preprocess(List<Token> tokens)
    {
        Log("Clearing comments and empty tokens from the passed tokens.");
        var processed = new List<Token>();
        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (token == null || token.Content.Length == 0 || token.Type == TokenType.Whitespace || token.Type == TokenType.Comment)
                continue; // Skip token.
            processed.Add(token);
        }
        Log("Condensing complex tokens.");
        var pass = new List<Token>();
        for (int i = 0; i < processed.Count; i++)
        {
            var token = processed[i];
            // Condense tokens

            pass.Add(token);
        }
        return pass;
    }
}
