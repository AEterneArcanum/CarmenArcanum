namespace Arcane.Carmen.Lexer;

public static class TokenEx
{
    public static string AsString(this List<Token> tokens)
    {
        string str = string.Empty;
        foreach (Token token in tokens) { str += token.Content + " "; }
        return str.TrimEnd();
    }

    public static string AsString(this IEnumerable<Token> tokens)
    {
        string str = string.Empty;
        foreach (Token token in tokens) { str += token.Content + " "; }
        return str.TrimEnd();
    }

    public static AST.Types.ASTPosition GetASTPosition(this IEnumerable<Token> tokens)
    {
        if (tokens is null || !tokens.Any()) return new(new(0, 0, ""), new(0, 0, ""));
        return new(tokens.First().Position, tokens.Last().Position);
    }

    public static Position GetPosition(this IEnumerable<Token> tokens)
    {
        if (tokens is null || !tokens.Any()) return new(0, 0, "");
        return tokens.First().Position;
    }
    
    public static Position GetPosition(this List<Token> tokens)
    {
        if (tokens is null || tokens.Count == 0) return new(0, 0, "");
        return tokens[0].Position;
    }

    public static decimal ConvertToWholeNumber(this Token token)
    {
        if (token.Type != TokenType.Number) { return 0; }
        string content = token.Content;
        if (decimal.TryParse(content, out decimal decvalue)) { return decvalue; }

        // Parse Binary
        if (content.ToLowerInvariant().StartsWith("0b"))
        {
            try
            {
                string binaryDigits = content[2..];
                if (binaryDigits.Length == 0) return 0;

                // Check for negative binary (uncommon but possible)
                bool isNegative = binaryDigits[0] == '-';
                if (isNegative)
                {
                    binaryDigits = binaryDigits[1..];
                }

                Int128 value = 0;
                foreach (char c in binaryDigits)
                {
                    value <<= 1;
                    value += c switch
                    {
                        '0' => 0,
                        '1' => 1,
                        _ => throw new FormatException($"Invalid binary digit: {c}")
                    };
                }
                return (decimal)(isNegative ? -value : value);
            }
            catch
            {
                return 0;
            }
        }
        // Parse Hexcode
        if (content.ToLowerInvariant().StartsWith("0x"))
        {
            string hexDigits = content[2..];
            if (hexDigits.Length == 0) return 0;

            // Check for negative hex
            bool isNegative = hexDigits[0] == '-';
            if (isNegative)
            {
                hexDigits = hexDigits[1..];
            }

            Int128 value = 0;
            foreach (char c in hexDigits)
            {
                value <<= 4;
                value += c switch
                {
                    >= '0' and <= '9' => c - '0',
                    >= 'a' and <= 'f' => c - 'a' + 10,
                    >= 'A' and <= 'F' => c - 'A' + 10,
                    _ => throw new FormatException($"Invalid hex digit: {c}")
                };
            }
            return (decimal)(isNegative ? -value : value);
        }
        // Parse Octal
        if (content.ToLowerInvariant().StartsWith("0o"))
        {
            try
            {
                string octalDigits = content[2..];
                bool isNegative = octalDigits[0] == '-';
                if (isNegative) octalDigits = octalDigits[1..];

                // Remove leading 0 if present (but not for single 0)
                if (octalDigits.Length > 1 && octalDigits[0] == '0')
                {
                    octalDigits = octalDigits[1..];
                }

                Int128 value = 0;
                foreach (char c in octalDigits)
                {
                    value <<= 3;
                    value += c switch
                    {
                        >= '0' and <= '7' => c - '0',
                        _ => throw new FormatException($"Invalid octal digit: {c}")
                    };
                }
                return (decimal)(isNegative ? -value : value);
            }
            catch
            {
                return 0;
            }
        }

        return 0;
    }
}
