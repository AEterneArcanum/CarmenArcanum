namespace Arcane.Carmen.Lexer;

/// <summary>
/// A generic token storage post lexagraphic parse.
/// </summary>
/// <param name="Content">Textual content in file.</param>
/// <param name="Line">Line found at.</param>
/// <param name="Column">Column found t.</param>
/// <param name="Type">Generic type of parsed content.</param>
public record Token(string Content, Position Position, TokenType Type) { 
    public override string ToString() { return Content; }
};
