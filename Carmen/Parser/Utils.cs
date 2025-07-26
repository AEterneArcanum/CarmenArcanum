using Arcane.Carmen.Lexer;

namespace Arcane.Carmen.Parser
{
    public static class Utils
    {
        public static bool IsKeyword(Token token, Keywords keyword)
        {
            return token.TryMatchKeyword(out var match) && match == keyword;
        }

        public static bool IsKeyword(Token token, Keywords[] keywords, out Keywords match)
        {
            if (token.TryMatchKeyword(out var mat))
            {
                match = mat;
                return keywords.Any(k => k == mat);
            }
            match = Keywords.Unknown;
            return false;
        }

        public static int NextTopLevelIndexOfKeywords(Token[] tokens, Keywords[] keywords, out Keywords match, int start = 0)
        {
            match = Keywords.Unknown;
            if (tokens == null || start < 0 || start >= tokens.Length)
                return -1;
            int depth = 0;
            for (int i = start; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (token.TryMatchKeyword(out var currentWord))
                {
                    switch (currentWord)
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

                    if (depth == 0 && keywords.Any(k => k == currentWord))
                    {
                        match = currentWord;
                        return i;
                    }
                }

            }
            return -1;
        }

        public static int NextTopLevelIndexOfKeyword(Token[] tokens, Keywords keyword, int start = 0)
        {
            if (tokens == null || start < 0 || start >= tokens.Length   )
                return -1;
            int depth = 0;
            for (int i = start; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (token.TryMatchKeyword(out var currentWord))
                {
                    switch (currentWord)
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

                    if (depth == 0 && currentWord == keyword) { return i; }
                }

            }
            return -1;
        }
    }
}
