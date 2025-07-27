using Arcane.Carmen.Lexer;

namespace Arcane.Carmen.Parser;

public record ParserError(string Function, string Message, Token[] Tokens);
