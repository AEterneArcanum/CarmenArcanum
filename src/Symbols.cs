using LLVMSharp;

namespace Arcane.Script.Carmen;

/// <summary>
/// Literated keywords and characters for use by the lexer.
/// </summary>
public static class Symbols
{
    public const string COMMA = ",";
    public const string COMMENT_END = "›";
    public const string COMMENT_START = "‹";
    public const string EOL = ".";
    public const string IDENTIFIER = "·"; // NAMER from shavian keyboard.
    public const string LITERAL_STRING_END = "\"";
    public const string LITERAL_STRING_START = "\"";
    public const string NEGATIVE_SIGN = "-";

    public const string BOOL = "𐑚𐑵𐑤";
    public const string NOT = "𐑯𐑪𐑑";
    public const string EQUAL = "𐑰𐑒𐑢𐑩𐑤";
    public const string TO = "𐑑𐑴";
    public const string LESSER = "𐑤𐑧𐑕𐑻";
    public const string GREATER = "𐑜𐑮𐑱𐑑𐑻";
    public const string THAN = "𐑞𐑩𐑯";
    public const string TRUE = "𐑑𐑤𐑵";
    public const string FALSE = "𐑓𐑩𐑤𐑕";
    public const string AND = "𐑨𐑯𐑛";
    public const string OR = "𐑹";
    public const string LABEL = "𐑝𐑻𐑮𐑕"; //'verse'
    public const string GOTO = "𐑕𐑦𐑙"; //'sing'
    public const string DISPLAY = "𐑛𐑦𐑕𐑐𐑤𐑲";
    public const string IS = "𐑦𐑟";
    public const string A = "𐑩";
    public const string INTEGER = "𐑦𐑯𐑑𐑧𐑡𐑻";
    public const string INT = "𐑦𐑯𐑑";
    public const string STRING = "𐑕𐑑𐑮𐑦𐑙";
    public const string STR = "𐑕𐑑𐑮";
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
    public const string THEN = "𐑞𐑧𐑯";
    public const string RECEIVED = "𐑮𐑩𐑕𐑰𐑝𐑛";
    public const string EITHER = "𐑲𐑞𐑻";
    public const string BOTH = "𐑚𐑴𐑔";

    public static bool MatchSymbol(string value, out TokenType token)
    {
        token = value switch
        {
            A => TokenType.A,
            AND => TokenType.AND,
            COMMA => TokenType.COMMA,
            BOOL => TokenType.BOOL,
            BOTH => TokenType.BOTH,
            DIFFERENCE => TokenType.DIFFERENCE,
            DISPLAY => TokenType.DISPLAY,
            EITHER => TokenType.EITHER,
            EQUAL => TokenType.EQUAL,
            EOL => TokenType.EOL,
            FALSE => TokenType.FALSE,
            GOTO => TokenType.GOTO,
            IF => TokenType.IF,
            INCREMENT => TokenType.INCREMENT,
            INT => TokenType.INTEGER, // Alias for INTEGER
            INTEGER => TokenType.INTEGER,
            INTO => TokenType.INTO,
            IS => TokenType.IS,
            LABEL => TokenType.LABEL,
            LESSER => TokenType.LESSER,
            MODULUS => TokenType.MODULUS,
            NOT => TokenType.NOT,
            OF => TokenType.OF,
            OR => TokenType.OR,
            PRODUCT => TokenType.PRODUCT,
            PUT => TokenType.PUT,
            QUOTIENT => TokenType.QUOTIENT,
            RECEIVED => TokenType.RECEIVED,
            STR => TokenType.STRING, // Alias for STRING
            STRING => TokenType.STRING,
            SUM => TokenType.SUM,
            THAN => TokenType.THAN,
            THE => TokenType.THE,
            THEN => TokenType.THEN,
            TO => TokenType.TO,
            TRUE => TokenType.TRUE,
            _ => _matchComplexToken(value)
        };
        return token != TokenType.UNKNOWN;
    }

    private static TokenType _matchComplexToken(string value)
    {
        if (value.StartsWith(LITERAL_STRING_START)
            && value.EndsWith(LITERAL_STRING_END))
            return TokenType.LITERAL_STRING;
        else if (value.StartsWith(IDENTIFIER))
            return TokenType.IDENTIFIER;
        else if (int.TryParse(value, out _))
            return TokenType.LITERAL_INTEGER;
        else
            return TokenType.UNKNOWN;    
    }
}
