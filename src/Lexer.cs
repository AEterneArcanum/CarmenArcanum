using System.Diagnostics;
using System.Text;

/// <summary>
/// Enumerated Keywords And Types
/// </summary>
public enum TokenType
{
    LABEL, GOTO, IDENTIFIER, DISPLAY_STRING, LITERAL_STRING, EOL, LITERAL_INTEGER,
    IS, A, INTEGER, STRING, PUT, INTO,
    // Adding boolean support for conditionals
    BOOL, NOT, EQUAL, LESS, GREATER, THAN, TRUE, FALSE, TO,

    AND, OR, COMMA, THE, OF, SUM, DIFFERENCE, QUOTIENT, PRODUCT, MODULUS,

    INCREMENT, IF, THEN
}

public record Token(TokenType Type, string Value, int Line, int Column);
/// <summary>
/// Literated keywords and characters for use by the lexer.
/// </summary>
public static class Symbols
{
    public const string BOOL = "𐑚𐑵𐑤";
    public const string NOT = "𐑯𐑪𐑑";
    public const string EQUAL = "𐑰𐑒𐑢𐑩𐑤";
    public const string TO = "𐑑𐑴";
    public const string LESS = "𐑤𐑧𐑕";
    public const string GREATER = "𐑜𐑮𐑱𐑑𐑻";
    public const string THAN = "𐑞𐑩𐑯";
    public const string TRUE = "𐑑𐑤𐑵";
    public const string FALSE = "𐑓𐑩𐑤𐑕";

    public const string AND = "𐑨𐑯𐑛";
    public const string OR = "𐑹";
    public const string COMMA = ",";

    public const string LABEL = "𐑝𐑻𐑮𐑕"; //'verse'
    public const string GOTO = "𐑕𐑦𐑙"; //'sing'
    public const string IDENTIFIER = "·"; // NAMER from shavian keyboard.
    public const string DISPLAY_STRING = "𐑛𐑦𐑕𐑐𐑤𐑲";
    public const string LITERAL_STRING_START = "\"";
    public const string LITERAL_STRING_END = "\"";
    public const string EOL = ".";
    public const string COMMENT_START = "‹";
    public const string COMMENT_END = "›";

    public const string IS = "𐑦𐑟";
    public const string A = "𐑩";
    public const string INTEGER = "𐑦𐑯𐑑𐑧𐑡𐑻";
    public const string STRING = "𐑕𐑑𐑮𐑦𐑙";
    public const string PUT = "𐑐𐑳𐑑";
    public const string INTO = "𐑦𐑯𐑑𐑴";
    public const string THE = "𐑞";
    public const string OF = "𐑴𐑓";

    public const string SUM = "𐑕𐑳𐑥";
    public const string DIFFERENCE = "𐑛𐑦𐑓𐑮𐑧𐑯𐑕";
    public const string QUOTIENT = "𐑒𐑢𐑴𐑖𐑧𐑯𐑑";
    public const string PRODUCT = "𐑐𐑮𐑩𐑛𐑳𐑒𐑑";
    public const string MODULUS = "𐑥𐑩𐑛𐑵𐑤𐑳𐑕";

    public const string INCREMENT = "𐑦𐑯𐑒𐑮𐑧𐑤𐑧𐑯𐑑";
    public const string IF = "𐑦𐑓";
    public const string THEN = "𐑞𐑯";
}

public static class Lexer
{
    public static List<Token> Tokenize(string input)
        => Tokenize(input.Contains('\n') ? input.Split("\n") : [input]);

    public static List<Token> Tokenize(string[] lines)
    {
        Debug.WriteLine("Initializing Tokenizer...");
        List<Token> tokens = [];
        StringBuilder buffer = new();
        bool inComment = false;
        bool inString = false;
        Debug.WriteLine("Interating through all chars...");
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
                && c != Symbols.IDENTIFIER[0]) // remove identifier symbol because it is attached to names
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
        TokenType? type = null;
        type = s switch
        {
            Symbols.DISPLAY_STRING => TokenType.DISPLAY_STRING,
            Symbols.EOL => TokenType.EOL,
            Symbols.LABEL => TokenType.LABEL,
            Symbols.GOTO => TokenType.GOTO,
            Symbols.IS => TokenType.IS,
            Symbols.A => TokenType.A,
            Symbols.INTEGER => TokenType.INTEGER,
            Symbols.STRING => TokenType.STRING,
            Symbols.PUT => TokenType.PUT,
            Symbols.INTO => TokenType.INTO,

            Symbols.BOOL => TokenType.BOOL,
            Symbols.NOT => TokenType.NOT,
            Symbols.EQUAL => TokenType.EQUAL,
            Symbols.TO => TokenType.TO,
            Symbols.LESS => TokenType.LESS,
            Symbols.GREATER => TokenType.GREATER,
            Symbols.THAN => TokenType.THAN,
            Symbols.TRUE => TokenType.TRUE,
            Symbols.FALSE => TokenType.FALSE,

            Symbols.AND => TokenType.AND,
            Symbols.OR => TokenType.OR,
            Symbols.COMMA => TokenType.COMMA,

            Symbols.THE => TokenType.THE,
            Symbols.OF => TokenType.OF,

            Symbols.SUM => TokenType.SUM,
            Symbols.DIFFERENCE => TokenType.DIFFERENCE,
            Symbols.QUOTIENT => TokenType.QUOTIENT,
            Symbols.PRODUCT => TokenType.PRODUCT,
            Symbols.MODULUS => TokenType.MODULUS,

            Symbols.IF => TokenType.IF,
            Symbols.THEN => TokenType.THEN,
            Symbols.INCREMENT => TokenType.INCREMENT,

            _ => null
        };

        // Try to match to literals and identifiers.
        if (type is null)
        {
            if (s.StartsWith(Symbols.LITERAL_STRING_START)
                && s.EndsWith(Symbols.LITERAL_STRING_END))
                type = TokenType.LITERAL_STRING;
            else if (s.StartsWith(Symbols.IDENTIFIER))
                type = TokenType.IDENTIFIER;
        }

        // Try to match the token to literal number or spelled digit
        if (type is null)
        {
            if (int.TryParse(s, out int r)) // Is an ordinary integer
            {
                type = TokenType.LITERAL_INTEGER;
            }
        }

        // Ensure a proper token type has been found.
        if (type is null)
            throw new NullReferenceException($"Symbol {s} not properly typed.");

        // Add the new token to the buffer.
        tokens.Add(new(type.Value, s, line, column - s.Length));
    }
}
