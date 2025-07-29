using Arcane.Carmen.Lexer;
using System.Runtime.CompilerServices;
namespace Arcane.Carmen.Parser;

public record CarmenToken(string Content, Position Position, TokenType Type, Keywords Keyword) : Token(Content, Position, Type);

public static class CarmenTokenEx
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsKeyword(this CarmenToken token, Keywords keyword)
    {
        return token.Keyword == keyword;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotKeyword(this CarmenToken token, Keywords keyword)
    {
        return token.Keyword != keyword;
    }

    public static int NextTopLevelIndexOf(this CarmenToken[] tokens, Keywords keyword, int start = 0, int initialDepth = 0)
    {
        if (tokens is null || tokens.Length == 0 || start < 0 || start >= tokens.Length)
            return -1;
        int depth = initialDepth;
        for (int i = start; i < tokens.Length; i++)
        {
            var token = tokens[i];

            switch (token.Keyword)
            {
                case Keywords.BlockStart:
                case Keywords.OpenParen:
                    depth++;
                    break;
                case Keywords.BlockEnd:
                case Keywords.CloseParen:
                    depth--;
                    break;
            }

            if (depth == 0 && token.Keyword == keyword) { return i; }
            if (depth < 0) break;
        }
        return -1;
    }

    public static int NextTopLevelIndexOf(this CarmenToken[] tokens, Keywords[] keywords, out Keywords match, int start = 0, int initialDepth = 0)
    {
        match = Keywords.Unknown;
        if (tokens == null || tokens.Length == 0 || start < 0 || start >= tokens.Length)
            return -1;
        int depth = initialDepth;
        for (int i = start; i < tokens.Length; i++)
        {
            var token = tokens[i];
            switch (token.Keyword)
            {
                case Keywords.BlockStart:
                case Keywords.OpenParen:
                    depth++;
                    break;
                case Keywords.BlockEnd:
                case Keywords.CloseParen:
                    depth--;
                    break;
            }
            if (depth == 0 && keywords.Any(k => k == token.Keyword))
            {
                match = token.Keyword;
                return i;
            }
            if (depth < 0) break;
        }
        return -1;
    }

    public static int[] TopLevelIndicesOf(this CarmenToken[] tokens, Keywords keyword)
    {
        List<int> indices = [];
        int depth = 0;
        for (int i = 0; i < tokens.Length; i++)
        {
            var token = tokens[i];

            switch (token.Keyword)
            {
                case Keywords.OpenParen:
                case Keywords.BlockStart: 
                    depth++;
                    break;
                case Keywords.BlockEnd:
                case Keywords.CloseParen:
                    depth--;
                    break;
            }

            if (depth == 0 && token.Keyword == keyword)
                indices.Add(i);
        }
        return [.. indices];
    }

    public static IEnumerable<CarmenToken[]> Split(this CarmenToken[] tokens, Keywords keyword)
    {
        if (tokens.Length == 0) return [];
        var indices = tokens.TopLevelIndicesOf(keyword);
        if (indices.Length == 0) return [tokens];
        var parts = new List<CarmenToken[]>();

        var lst = 0;
        for (int i = 0; i < indices.Length; i++)
        {
            parts.Add(tokens[lst..indices[i]]);
            lst = indices[i] + 1;
        }
        parts.Add(tokens[lst..]);

        return [.. parts];
    }
}
