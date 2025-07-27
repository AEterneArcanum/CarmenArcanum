using Arcane.Carmen.Lexer;

namespace Arcane.Carmen.Parser;

public class ParserException( string function, string message, 
    IReadOnlyCollection<Token> tokens) 
    : Exception(message)
{
    public string Function { get; init; } = function;
    public IReadOnlyCollection<Token> Tokens { get; init; } = tokens;
}
