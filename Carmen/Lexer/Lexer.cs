using System.Text;

namespace Arcane.Carmen.Lexer;

/// <summary>
/// The sole purpose of this lexer is to transform raw text into the simplest form of token maintainable.
/// </summary>
public static class Lexer
{
    private static void AddToken(List<Token> tokens, StringBuilder stringBuilder, ref LexerState state, TokenType type)
    {
        tokens.Add(new(stringBuilder.ToString(), state.GetPosition(), type));
        stringBuilder.Clear();
    }
    private static void HandleComment(char c, ref LexerState state, StringBuilder commentBuilder, List<Token> tokens)
    {
        commentBuilder.Append(c);
        if (c == '\n') {
            tokens.Add(new(commentBuilder.ToString(), state.GetPosition(), TokenType.Comment));
            commentBuilder.Clear();
            state.InComment = false;
        }
    }
    private static void HandleMultilineComment(string raw, ref int i, ref LexerState state, StringBuilder commentBuilder, List<Token> tokens)
    {
        char ch = raw[i];
        commentBuilder.Append(ch);

        if (ch == '*' && i + 1 < raw.Length && raw[i + 1] == '\\')
        {
            commentBuilder.Append(raw[++i]);
            state.Column++;
            tokens.Add(new(commentBuilder.ToString(), state.GetPosition(), TokenType.Comment));
            commentBuilder.Clear();
            state.InMultilineComment = false;
        }
    }
    private static void HandleString(string raw, ref int i, ref LexerState state, StringBuilder stringBuilder, List<Token> tokens)
    {
        char c = raw[i];
        stringBuilder.Append(c);
        if (c == '\\' && i + 1 < raw.Length) 
        {
            stringBuilder.Append(raw[++i]);
            state.Column++;
        }
        else if (c == '"')
        {
            AddToken(tokens, stringBuilder, ref state, TokenType.String);
            stringBuilder.Clear();
            state.InString = false;
        }
    }
    private static void HandleChar(string raw, ref int i, ref LexerState state, StringBuilder stringBuilder, List<Token> tokens)
    {
        char c = raw[i];
        stringBuilder.Append(c);
        if (c == '\\' && i + 1 < raw.Length)
        {
            stringBuilder.Append(raw[++i]);
            state.Column++;
        }
        else if (c == '\'')
        {
            AddToken(tokens, stringBuilder, ref state, TokenType.Char);
            stringBuilder.Clear();
            state.InChar = false;
        }
    }
    private static bool TryEnterComment(string raw, ref int i, ref LexerState state, StringBuilder stringBuilder, List<Token> tokens)
    {
        char c = raw[i];
        if (c == '\\' && i + 1 < raw.Length)
        {
            if (raw[i+1]=='\\')
            {
                state.InComment = true;
                state.Column++; i++;
                return true;
            }
            else if (raw[i+1]=='*')
            {
                state.InMultilineComment = true;
                state.Column++; i++;
                return true;
            }
        }
        return false;
    }
    private static bool TryHandleWhiteSpace(char c, ref LexerState state, StringBuilder stringBuilder, List<Token> tokens)
    {
        if (char.IsWhiteSpace(c))
        {
            //FlushBuffer(stringBuilder, tokens, ref state);
            stringBuilder.Append(c);
            AddToken(tokens, stringBuilder, ref state, TokenType.Whitespace);
            return true;
        }
        return false;
    }
    private static void HandlePunctuation(string raw, ref int i, ref LexerState state, StringBuilder stringBuilder, List<Token> tokens)
    {
        char c = raw[i];
        switch (c)
        {
            case '\'':
                stringBuilder.Append(c);
                state.InChar = true;
                break;
            case '"':
                stringBuilder.Append(c);
                state.InString = true;
                break;
            case '?':
                if (i + 1 < raw.Length && raw[i + 1] == '?') // ??
                {
                    stringBuilder.Append(c);
                    stringBuilder.Append(raw[i + 1]);
                    i++; state.Column++;
                    AddToken(tokens, stringBuilder, ref state, TokenType.Operators);
                    break;
                }
                goto DEFAULT;
            case '-':
                if (i + 1 < raw.Length && raw[i + 1] == '=')
                    goto EQUALS;
                HandleNumeric(raw, ref i, ref state, stringBuilder, tokens);
                break;
            case '$': // Allow cash and hash and at as id prefixes
            case '@':
            case '#':
            case '·': // Shavian namer symbol belongs to following word.
                HandleIdentifier(raw, ref i, ref state, stringBuilder, tokens);
                break;
            case '&':
                if (i + 1 < raw.Length && raw[i + 1] == '=')
                    goto EQUALS;

                //FlushBuffer(stringBuilder, tokens, ref state);
                if (i + 1 < raw.Length && raw[i + 1] == '&') // <<
                {
                    stringBuilder.Append(c);
                    stringBuilder.Append(raw[i + 1]);
                    i++; state.Column++;
                    AddToken(tokens, stringBuilder, ref state, TokenType.Operators);
                    break;
                }
                goto DEFAULT;
            case '|':
                //FlushBuffer(stringBuilder, tokens, ref state);
                if (i + 1 < raw.Length && raw[i + 1] == '|') // <<
                {
                    stringBuilder.Append(c);
                    stringBuilder.Append(raw[i + 1]);
                    i++; state.Column++;
                    AddToken(tokens, stringBuilder, ref state, TokenType.Operators);
                    break;
                }
                goto DEFAULT;
            case '<':
                //FlushBuffer(stringBuilder, tokens, ref state);
                if (i + 1 < raw.Length && raw[i + 1] == '<') // <<
                {
                    stringBuilder.Append(c);
                    stringBuilder.Append(raw[i + 1]);
                    i++; state.Column++;
                    AddToken(tokens, stringBuilder, ref state, TokenType.Operators);
                    break;
                }
                goto EQUALS;
            case '>':
                //FlushBuffer(stringBuilder, tokens, ref state);
                if (i + 1 < raw.Length && raw[i + 1] == '>') // >>
                {
                    stringBuilder.Append(c);
                    stringBuilder.Append(raw[i + 1]);
                    i++; state.Column++;
                    AddToken(tokens, stringBuilder, ref state, TokenType.Operators);
                    break;
                }
                goto EQUALS;
            case '!': // compound equals
            case '+':
            case '*':
            case '/':
            case '=': EQUALS:
                //FlushBuffer(stringBuilder, tokens, ref state);
                if (i + 1 < raw.Length && raw[i + 1] == '=')
                {
                    stringBuilder.Append(c);
                    stringBuilder.Append(raw[i + 1]);
                    i++; state.Column++;
                    AddToken(tokens, stringBuilder, ref state, TokenType.Operators);
                    break;
                }
                stringBuilder.Append(c);
                TokenType t2 = IsOperator(c) ? TokenType.Operators : TokenType.Punctuation;
                AddToken(tokens, stringBuilder, ref state, t2);
                break;
            default: DEFAULT:
                //FlushBuffer( stringBuilder, tokens, ref state );
                stringBuilder.Append(c);
                TokenType type = IsOperator(c) ? TokenType.Operators : TokenType.Punctuation;
                AddToken(tokens, stringBuilder, ref state, type);
                break;
        }
    }
    private static void HandleNumeric(string raw, ref int i, ref LexerState state, StringBuilder stringBuilder, List<Token> tokens)
    {
        char c = raw[i];
        if (c == '-')
        {
            stringBuilder.Append(c);
            if (i + 1 >= raw.Length || !IsDigit(raw[i + 1]))
            {
                AddToken(tokens, stringBuilder, ref state, TokenType.Operators);
                stringBuilder.Clear();
                return;
            }
            i++;
            state.Column++;
            c = raw[i];
        }
        stringBuilder.Append(c);

        if (c == '0' && i + 1 < raw.Length)
        {
            char nxt = raw[i + 1];
            if (nxt == 'x' || nxt == 'X')
            {
                stringBuilder.Append(nxt);
                i += 2;
                state.Column += 2;
                while (i < raw.Length && char.IsAsciiHexDigit(raw[i]))
                {
                    stringBuilder.Append(raw[i++]);
                    state.Column++;
                }
                AddToken(tokens, stringBuilder, ref state, TokenType.Number);
                stringBuilder.Clear();
                i--;
                return;
            }
            else if (nxt == 'b' || nxt == 'B')
            {
                stringBuilder.Append(nxt);
                i += 2;
                state.Column += 2;
                while (i < raw.Length && IsBinDigit(raw[i]))
                {
                    stringBuilder.Append(raw[i++]);
                    state.Column++;
                }
                AddToken(tokens, stringBuilder, ref state, TokenType.Number);
                stringBuilder.Clear();
                i--;
                return;
            }
            else if (nxt == 'o' || nxt == 'O')
            {
                stringBuilder.Append(nxt);
                i += 2;
                state.Column += 2;
                while (i < raw.Length && IsOctDigit(raw[i]))
                {
                    stringBuilder.Append(raw[i++]);
                    state.Column++;
                }
                AddToken(tokens, stringBuilder, ref state, TokenType.Number);
                stringBuilder.Clear();
                i--;
                return;
            }
        }

        bool hasDecimal = false;
        while (i + 1 < raw.Length)
        {
            char nxt = raw[i + 1];
            if (IsDigit(nxt))
            {
                stringBuilder.Append(nxt);
                i++;
                state.Column++;
            }
            else if (nxt == '.' && !hasDecimal && i + 2 < raw.Length && IsDigit(raw[i + 2]))
            {
                hasDecimal = true;
                stringBuilder.Append(nxt);
                i++;
                state.Column++;
            }
            else
            {
                break;
            }
        }

        AddToken(tokens, stringBuilder, ref state, TokenType.Number);
        stringBuilder.Clear();
    }
    private static void HandleIdentifier(string raw, ref int i, ref LexerState state, StringBuilder stringBuilder, List<Token> tokens)
    {
        //FlushBuffer(stringBuilder, tokens, ref state);
        stringBuilder.Append(raw[i]);

        while (i + 1 < raw.Length && (IsLetter(raw[i + 1]) || IsDigit(raw[i + 1]) || raw[i + 1] == '_'))
        {
            state.Column++;
            stringBuilder.Append(raw[++i]);
        }

        AddToken(tokens, stringBuilder, ref state, TokenType.Identifier);
    }
    //private static void FlushBuffer(StringBuilder buffer, List<Token> store, ref LexerState state, TokenType defType = TokenType.Code)
    //{
    //    if (buffer.Length == 0) return;
    //    store.Add(new(buffer.ToString(), state.GetPosition(), defType));
    //    buffer.Clear();
    //}

    private static bool IsOperator(char c) => 
        c == '+' || c == '-' || c == '*' || c == '/' || c == '=' || 
        c == '<' || c == '>' || c == '!' || c == '&' || c == '|';
    private static bool IsOctDigit(char c) => c >= 0 && c <= 7;
    private static bool IsBinDigit(char c) => c == '0' || c == '1';
    private static bool IsDigit(char c) => char.IsDigit(c); // In case i want to add unicode digits in the future.
    private static bool IsLetter(char c) => char.IsLetter(c) || Shavian.IsLetter(c);
    private static bool IsPunctuation(char c) => char.IsPunctuation(c) || Shavian.IsPunctuation(c);

    /// <summary>
    /// Convert raw text to individual tokens.
    /// </summary>
    /// <param name="raw">Raw text.</param>
    /// <returns>Token(s).</returns>
    public static List<Token> Parse(string raw, string filename)
    {
        List<Token> tokens = [];
        StringBuilder sb = new();
        StringBuilder cb = new();
        LexerState state = new() 
        { 
            Filename = filename 
        };

        for (int i = 0; i < raw.Length; i++)
        {
            char ch = raw[i];
            state.UpdatePosition(ch);

            if (state.InMultilineComment)
            {
                HandleMultilineComment(raw, ref i, ref state, cb, tokens);
                continue;
            }
            else if (state.InComment)
            {
                HandleComment(ch, ref state, cb, tokens);
                continue;
            }
            else if (state.InString)
            {
                HandleString(raw, ref i, ref state, sb, tokens);
                continue;
            }
            else if (state.InChar)
            {
                HandleChar(raw, ref i, ref state, sb, tokens); 
                continue;
            }

            if (TryEnterComment(raw, ref i, ref state, sb, tokens))
            {
                //FlushBuffer(sb, tokens, ref state);
                continue;
            }

            if (TryHandleWhiteSpace(ch, ref state, sb, tokens))
                continue;

            if (char.IsDigit(ch))
            {
                //FlushBuffer(sb, tokens, ref state);
                HandleNumeric(raw, ref i, ref state, sb, tokens);
                continue;
            }

            if (IsPunctuation(ch))
            {
                //FlushBuffer(sb, tokens, ref state);
                HandlePunctuation(raw, ref i, ref state, sb, tokens);
                continue;
            }

            if (IsLetter(ch))
            {
                HandleIdentifier(raw, ref i, ref state, sb, tokens);
                continue;
            }

            sb.Append(ch);
        }
        //FlushBuffer(sb, tokens, ref state);

        if (sb.Length > 0)
        {
            tokens.Add(new(sb.ToString(), state.GetPosition(), TokenType.Code));
        }
        if (cb.Length > 0) {
            tokens.Add(new(cb.ToString(), state.GetPosition(), TokenType.Comment));
        }
        return tokens;
    }

}
