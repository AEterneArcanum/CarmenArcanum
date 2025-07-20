using System.Text;

namespace Arcane.Script.Carmen;

public record Token(TokenType Type, string Value, int Line, int Column);

public static class Lexer
{
    public static List<Token> Tokenize(string input)
        => Tokenize(input.Contains('\n') ? input.Split("\n") : [input]);

    public static List<Token> Tokenize(string[] lines)
    {
        List<Token> tokens = [];
        StringBuilder buffer = new();
        bool inComment = false;
        bool inString = false;
        int i = 0; int column = 0;
        for (; i < lines.Length; i++) // Iterate through lines
        {
            column = 0;
            for (; column < lines[i].Length; column++) // Iterate chars in line
            {
                char c = lines[i][column];

                if (inComment) // Comment handling ignore all contents
                {
                    if (c == Symbols.COMMENT_END[0]) // Exit comment handling
                        inComment = false;
                    continue; // No more processing for this char
                }
                if (inString) // String handling all chars get added to buffer
                {
                    buffer.Append(c);
                    if (c == Symbols.LITERAL_STRING_END[0])
                    {
                        _flushBuffer(buffer, tokens, i, column); // Send token including literal string symbols
                        inString = false; // Leave string handling
                    }
                    continue; // No more processing for this char
                }

                if (c == Symbols.COMMENT_START[0]) // Enter comment handling.
                {
                    _flushBuffer(buffer, tokens, i, column); // Clear buffer
                    inComment = true;
                    continue; // No more handling for this char
                }
                if (c == Symbols.LITERAL_STRING_START[0]) // Enter string handling
                {
                    _flushBuffer(buffer, tokens, i, column); // Clear buffer
                    inString = true;
                    buffer.Append(c); // Include string start symbol
                    continue; // No more handling for this char
                }
                if (char.IsWhiteSpace(c)) // On whitespace push buffer - move on
                {
                    _flushBuffer(buffer, tokens, i, column);
                    continue; // No more handling for this char
                }
                if (Shavian.IsPunctuation(c)
                && c != Symbols.IDENTIFIER[0]  // remove identifier symbol because it is attached to names
                && c != Symbols.NEGATIVE_SIGN[0]) // Going to remove negative sign as it is attached to integers
                {
                    _flushBuffer(buffer, tokens, i, column);
                    buffer.Append(c);
                    _flushBuffer(buffer, tokens, i, column);
                    continue;
                }

                buffer.Append(c);
            }
            _flushBuffer(buffer, tokens, i, column);
        }
        _flushBuffer(buffer, tokens, i, column);
        return tokens;
    }

    private static void _flushBuffer(StringBuilder buffer, List<Token> tokens,
            int line, int column)
    {
        if (buffer.Length == 0) // Check if buffer is empty
            return; // Buffer is empty
        string s = buffer.ToString(); // Get buffer as string
        buffer.Clear(); // Ensure the buffer is emptied

        // Try to match to keywords and structural symbols
        if (!Symbols.MatchSymbol(s, out var type))
            throw new NullReferenceException($"Symbol {s} not properly typed.");
        tokens.Add(new(type, s, line, column - s.Length));
    }
}
