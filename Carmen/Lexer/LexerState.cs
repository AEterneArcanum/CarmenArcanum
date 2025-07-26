namespace Arcane.Carmen.Lexer;

internal struct LexerState
{
    public int Line { get; set; }
    public int Column { get; set; }
    public bool InComment { get; set; }
    public bool InMultilineComment { get; set; }
    public bool InString { get; set; }
    public bool InChar { get; set; }
    public string Filename { get; init; }

    public void UpdatePosition(char c)
    {
        if (c == '\n')
        {
            Line++;
            Column = 1;
        }
        Column++;
    }

    public Position GetPosition() => new(Line, Column, Filename);
}