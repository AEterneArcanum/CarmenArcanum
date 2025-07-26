namespace Arcane.Carmen.Lexer;

/// <summary>
/// Basic token types.
/// </summary>
public enum TokenType
{
    Code,
    Identifier,
    String,
    Number,
    Char,
    Comment,
    Whitespace,
    Punctuation,
    Operators,
}