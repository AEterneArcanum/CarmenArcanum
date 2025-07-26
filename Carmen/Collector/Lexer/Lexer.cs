using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.Lexer
{
    public class Lexer : ILexer
    {

        private ISymbolMatcher SymbolMatcher;
        private ITokenMatcher TokenMatcher;

        public Lexer(ISymbolMatcher symbolMatcher, ITokenMatcher tokenMatcher)
        {
            SymbolMatcher = symbolMatcher;
            TokenMatcher = tokenMatcher;
        }

        private void FlushBuffer(StringBuilder buffer, List<Token> store, int line, int column)
        {
            if (buffer.Length == 0) return;
            string tok = buffer.ToString();
            buffer.Clear();

            if (TokenMatcher.TryMatchKeyword(tok, out var type) ||
                TokenMatcher.TryMatchComplex(tok, out type)) { }
            
            store.Add(new(type, tok, line, column));
        }

        public Token[] Lex(string raw)
        {
            List<Token> tokens = new List<Token>();
            StringBuilder sb = new StringBuilder();

            bool inComment = false;
            bool inMultilineComment = false;
            bool inString = false;
            bool inChar = false;

            int line = 1;
            int column = 1;

            for (int i = 0; i < raw.Length; i++)
            {
                char ch = raw[i];

                if (ch == SymbolMatcher.NewLineChar)
                {
                    line++;
                    column = 0;
                }
                column++;

                if (inMultilineComment)
                {
                    if (ch == SymbolMatcher.MultilineCommentInnerChar &&
                        i + 1 < raw.Length &&
                        raw[i + 1] == SymbolMatcher.MultilineCommentOuterChar)
                    {
                        inMultilineComment = false;
                        i++;
                    }
                    continue;
                }
                if (inComment)
                {
                    if (ch == SymbolMatcher.NewLineChar)
                        inComment = false;
                    continue;
                }
                if (inString)
                {
                    sb.Append(ch);
                    if (ch == SymbolMatcher.StringEscapeChar)
                    {
                        if (i + 1 < raw.Length)
                        {
                            i++;
                            column++;
                            sb.Append(ch);
                        }
                        continue;
                    }
                    if (ch == SymbolMatcher.StringLiteralChar)
                    {
                        FlushBuffer(sb, tokens, line, column);
                        inString = false;
                    }
                    continue;
                }
                if (inChar)
                {
                    sb.Append(ch);
                    if (ch == SymbolMatcher.CharEscapeChar)
                    {
                        if (i + 1 < raw.Length)
                        {
                            i++;
                            column++;
                            sb.Append(ch);
                        }
                        continue;
                    }
                    if (ch == SymbolMatcher.CharLiteralChar)
                    {
                        FlushBuffer( sb, tokens, line, column);
                        inChar = false;
                    }
                    continue;
                }

                if (ch == SymbolMatcher.CommentOuterChar &&
                    i + 1 < raw.Length &&
                    raw[i + 1] == SymbolMatcher.CommentInnerChar)
                {
                    FlushBuffer (sb, tokens, line, column);
                    inComment = true;
                    i++;
                    continue;
                }
                if (ch == SymbolMatcher.MultilineCommentOuterChar &&
                    i + 1 < raw.Length &&
                    raw[i+1] == SymbolMatcher.MultilineCommentInnerChar)
                {
                    FlushBuffer(sb, tokens, line, column);
                    inMultilineComment = true;
                    i++;
                    continue;
                }
                if (ch == SymbolMatcher.CharLiteralChar)
                {
                    FlushBuffer(sb, tokens, line, column);
                    inChar = true;
                    sb.Append(ch);
                    continue;
                }

                if (SymbolMatcher.IsWhiteSpace(ch))
                {
                    FlushBuffer(sb, tokens, line, column);
                    continue;
                }

                if (SymbolMatcher.IsDigit(ch))
                {
                    FlushBuffer(sb, tokens, line, column);
                    sb.Append(ch);

                    if (ch == SymbolMatcher.CharZero &&
                        i + 1 < raw.Length)
                    {
                        if (raw[i+1] == SymbolMatcher.HexPrefix)
                        {
                            i++;
                            sb.Append(raw[i]);
                            while (i + 1 < raw.Length &&
                                SymbolMatcher.IsHexDigit(raw[i + 1]))
                            {
                                i++;
                                sb.Append(raw[i]);
                            }
                            FlushBuffer(sb, tokens, line, column);
                            continue;
                        }
                        else if (raw[i+1] == SymbolMatcher.BinaryPrefix)
                        {
                            i++; sb.Append(raw[i]);
                            while (i + 1 < raw.Length &&
                                SymbolMatcher.IsBinaryDigit(raw[i + 1]))
                            {
                                i++;
                                sb.Append(raw[i]);
                            }
                            FlushBuffer(sb, tokens, line, column);
                            continue;
                        }
                    }

                    while (i + 1 < raw.Length && SymbolMatcher.IsDigit(raw[i + 1])
                        || raw[i+1] == SymbolMatcher.DecimalChar &&
                        i + 2 < raw.Length && SymbolMatcher.IsDigit(raw[i+2]))
                    {
                        i++;
                        sb.Append(raw[i]);
                    }
                    FlushBuffer (sb, tokens, line, column);
                    continue;
                }

                if (SymbolMatcher.IsPunctuation(ch))
                {
                    FlushBuffer(sb, tokens, line, column);
                    sb.Append(ch);
                    FlushBuffer(sb, tokens, line, column);
                    continue;
                }

                sb.Append(ch);
            }
            FlushBuffer( sb, tokens, line, column);

            tokens = CondenseIdentifiers(tokens);
            tokens = CondenseNumbers(tokens);
            tokens = CondenseKeywords(tokens);

            return [..tokens];
        }

        private List<Token> CondenseIdentifiers(List<Token> tokens)
        {
            var condensed = new List<Token>();

            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                if (token.Type == TokenType.AliasIdentifier && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.Unknown)
                    {
                        condensed.Add(new(TokenType.AliasIdentifier, token.Raw + tokens[i + 1].Raw, token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue; // Continue to the next token without adding the current one.
                    }
                    // solo alias symbol replace with wildcard token
                    condensed.Add(new(TokenType.KeywordWildcard, "_", token.Line, token.Column));
                    continue;
                }

                if (token.Type == TokenType.FuncIdentifier && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.Unknown)
                    {
                        condensed.Add(new(TokenType.FuncIdentifier, token.Raw + tokens[i + 1].Raw, token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue; // Continue to the next token without adding the current one.
                    }
                }

                if (token.Type == TokenType.LabelIdentifier && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.Unknown)
                    {
                        condensed.Add(new(TokenType.LabelIdentifier, token.Raw + tokens[i + 1].Raw, token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue; // Continue to the next token without adding the current one.
                    }
                    else
                    {
                        condensed.Add(new(TokenType.PunctuationColon, token.Raw, token.Line, token.Column));
                        // replace only the bad id with colon punctuation
                        continue;
                    }
                }

                if (token.Type == TokenType.StructIdentifier && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.Unknown)
                    {
                        condensed.Add(new(TokenType.StructIdentifier, token.Raw + tokens[i + 1].Raw, token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue; // Continue to the next token without adding the current one.
                    }
                }

                if (token.Type == TokenType.VariableIdentifier && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.Unknown)
                    {
                        condensed.Add(new(TokenType.VariableIdentifier, token.Raw + tokens[i + 1].Raw, token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue; // Continue to the next token without adding the current one.
                    }
                }

                condensed.Add(token); // Add the current token to the condensed list.
            }

            return condensed;
        }

        private List<Token> CondenseKeywords(List<Token> tokens)
        {
            var condensed = new List<Token>();
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                if (token.Type == TokenType.Program && i + 1 < tokens.Count && tokens[i+1].Type == TokenType.PunctuationColon)
                {
                    condensed.Add(new(TokenType.EntryPoint, "program:",  token.Line, token.Column));
                    i++;
                    continue;
                }

                if (token.Type == TokenType.KeywordFin)
                {
                    condensed.Add(new(TokenType.BlockEnd, "fin", token.Line, token.Column));
                    continue; // Skip the rest of the loop for this token.
                }

                if (token.Type == TokenType.KeywordExecute && i + 3 < tokens.Count &&
                    tokens[i + 1].Type == TokenType.KeywordThe &&
                    tokens[i + 2].Type == TokenType.KeywordFollowing &&
                    tokens[i + 3].Type == TokenType.PunctuationSemicolon)
                {
                    condensed.Add(new(TokenType.BlockStart, "execute the following;", token.Line, token.Column));
                    i += 3; // Skip the next three tokens as they have been combined.
                    continue; // Skip the rest of the loop for this token.
                }

                if (token.Type == TokenType.KeywordThe && i + 2 < tokens.Count)
                {
                    if (tokens[i + 2].Type == TokenType.KeywordOf)
                    {
                        switch (tokens[i + 1].Type)
                        {
                            case TokenType.KeywordSum:
                                condensed.Add(new(TokenType.KeywordTheSumOf, "the sum of", token.Line, token.Column));
                                i += 2; // Skip the next two tokens as they have been combined.
                                continue;
                            case TokenType.KeywordDifference:
                                condensed.Add(new(TokenType.KeywordTheDifferenceOf, "the difference of", token.Line, token.Column));
                                i += 2; // Skip the next two tokens as they have been combined.
                                continue;
                            case TokenType.KeywordProduct:
                                condensed.Add(new(TokenType.KeywordTheProductOf, "the product of", token.Line, token.Column));
                                i += 2; // Skip the next two tokens as they have been combined.
                                continue;
                            case TokenType.KeywordQuotient:
                                condensed.Add(new(TokenType.KeywordTheQuotientOf, $"the quotient of", token.Line, token.Column));
                                i += 2; // Skip the next two tokens as they have been combined.
                                continue;
                            case TokenType.KeywordAddress:
                                condensed.Add(new(TokenType.OperatorTheAddressOf, "the address of", token.Line, token.Column));
                                i += 2; // Skip the next two tokens as they have been combined.
                                continue;
                            default: // Do nothing, keep the original tokens.
                                break;
                        }
                    }
                }
                else if (token.Type == TokenType.KeywordShifted && i + 2 < tokens.Count)
                {
                    if ((tokens[i + 1].Type == TokenType.KeywordLeft || tokens[i + 1].Type == TokenType.KeywordRight)
                        && tokens[i + 2].Type == TokenType.KeywordBy)
                    {
                        if (tokens[i + 1].Type == TokenType.KeywordLeft)
                        {
                            condensed.Add(new Token(TokenType.OperatorShiftedLeftBy, "shifted left by", token.Line, token.Column));
                            i += 2; // Skip the next two tokens as they have been combined.
                            continue;
                        }
                        else
                        {
                            condensed.Add(new Token(TokenType.OperatorShiftedRightBy, "shifted right by", token.Line, token.Column));
                            i += 2; // Skip the next two tokens as they have been combined.
                            continue;
                        }
                    }
                }
                else if (token.Type == TokenType.KeywordRotated && i + 2 < tokens.Count)
                {
                    if ((tokens[i + 1].Type == TokenType.KeywordLeft || tokens[i + 1].Type == TokenType.KeywordRight)
                        && tokens[i + 2].Type == TokenType.KeywordBy)
                    {
                        if (tokens[i + 1].Type == TokenType.KeywordLeft)
                        {
                            condensed.Add(new Token(TokenType.OperatorRotatedLeftBy, "rotated left by", token.Line, token.Column));
                            i += 2; // Skip the next two tokens as they have been combined.
                            continue;
                        }
                        else
                        {
                            condensed.Add(new Token(TokenType.OperatorRotatedRightBy, "rotated right by", token.Line, token.Column));
                            i += 2; // Skip the next two tokens as they have been combined.
                            continue;
                        }
                    }
                }
                else if (token.Type == TokenType.PunctuationApostrophe && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.PunctuationS)
                    {
                        condensed.Add(new(TokenType.OperatorMemberAccess, "'s", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue; // Continue to the next token without adding the current one.
                    }
                }
                else if (token.Type == TokenType.KeywordBeginning && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordAt)
                    {
                        condensed.Add(new Token(TokenType.KeywordBeginningAt, "beginning at", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordEnding && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordAt)
                    {
                        condensed.Add(new Token(TokenType.KeywordEndingAt, "ending at", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordBitwise && i + 1 < tokens.Count)
                {
                    switch (tokens[i + 1].Type)
                    {
                        case TokenType.KeywordAnd:
                            condensed.Add(new Token(TokenType.OperatorBitwiseAnd, "bitwise and", token.Line, token.Column));
                            i++; // Skip the next token as it has been combined.
                            continue;
                        case TokenType.KeywordOr:
                            condensed.Add(new Token(TokenType.OperatorBitwiseOr, "bitwise or", token.Line, token.Column));
                            i++; // Skip the next token as it has been combined.
                            continue;
                        case TokenType.OperatorXor:
                            condensed.Add(new Token(TokenType.OperatorBitwiseXor, "bitwise xor", token.Line, token.Column));
                            i++; // Skip the next token as it has been combined.
                            continue;
                        case TokenType.KeywordNot:
                            condensed.Add(new Token(TokenType.OperatorBitwiseNot, "bitwise not", token.Line, token.Column));
                            i++; // Skip the next token as it has been combined.
                            continue;
                        default:
                            break; // Do nothing, keep the original tokens.
                    }
                }
                else if (token.Type == TokenType.KeywordConcatenated && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordWith)
                    {
                        condensed.Add(new Token(TokenType.OperatorConcatenatedWith, "concatenated with", token.Line, token.Column));
                        i++;
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordIs && i + 1 < tokens.Count)
                {
                    if (i + 2 < tokens.Count)
                    {
                        if (tokens[i + 1].Type == TokenType.KeywordNot && tokens[i + 2].Type == TokenType.LiteralNull)
                        {
                            condensed.Add(new Token(TokenType.KeywordIsNotNull, "is not null", token.Line, token.Column));
                            i += 2; // Skip the next two tokens as they have been combined.
                            continue;
                        }
                        else if (tokens[i + 1].Type == TokenType.KeywordOf && tokens[i + 2].Type == TokenType.KeywordType)
                        {
                            condensed.Add(new Token(TokenType.KeywordIsOfType, "is of type", token.Line, token.Column));
                            i += 2; // Skip the next two tokens as they have been combined.
                            continue;
                        }
                    }

                    if (tokens[i + 1].Type == TokenType.KeywordA)
                    {
                        condensed.Add(new Token(TokenType.KeywordIsA, "is a", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                    else if (tokens[i + 1].Type == TokenType.LiteralNull)
                    {
                        condensed.Add(new Token(TokenType.KeywordIsNull, "is null", token.Line, token.Column));
                        i++;
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordAs && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordA)
                    {
                        condensed.Add(new Token(TokenType.KeywordAsA, "as a", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordEqual && i + 1 < tokens.Count)
                {

                    if (tokens[i + 1].Type == TokenType.KeywordTo)
                    {
                        condensed.Add(new Token(TokenType.KeywordEqualTo, "equal to", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.PunctuationComma && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordAnd)
                    {
                        condensed.Add(new Token(TokenType.KeywordCommaAnd, ", and", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                    // , if null then
                    else if (i + 3 < tokens.Count
                        && tokens[i + 1].Type == TokenType.KeywordIf
                        && tokens[i + 2].Type == TokenType.LiteralNull
                        && tokens[i + 3].Type == TokenType.KeywordThen)
                    {
                        condensed.Add(new Token(TokenType.OperatorNullCoalesce, ", if null then", token.Line, token.Column));
                        i += 3; // Skip the next three tokens as they have been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordFrom && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordLast)
                    {
                        condensed.Add(new Token(TokenType.KeywordFromLast, "from last", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordExclusive && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordOr)
                    {
                        condensed.Add(new Token(TokenType.OperatorXor, "xor", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordGreater && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordThan)
                    {
                        // Try to condense a following "or equal to" if present.
                        if (i + 4 < tokens.Count &&
                            tokens[i + 2].Type == TokenType.KeywordOr &&
                            tokens[i + 3].Type == TokenType.KeywordEqual &&
                            tokens[i + 4].Type == TokenType.KeywordTo)
                        {
                            condensed.Add(new Token(TokenType.OperatorGreaterThanOrEqualTo, "greater than or equal to", token.Line, token.Column));
                            i += 4; // Skip the next four tokens as they have been combined.
                            continue;
                        }

                        condensed.Add(new Token(TokenType.OperatorGreaterThan, "greater than", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordLess && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordThan)
                    {
                        // Try to condense with a following "or equal to" if present.
                        if (i + 4 < tokens.Count &&
                            tokens[i + 2].Type == TokenType.KeywordOr &&
                            tokens[i + 3].Type == TokenType.KeywordEqual &&
                            tokens[i + 4].Type == TokenType.KeywordTo)
                        {
                            condensed.Add(new Token(TokenType.OperatorLessThanOrEqualTo, "less than or equal to", token.Line, token.Column));
                            i += 4; // Skip the next four tokens as they have been combined.
                            continue;
                        }


                        condensed.Add(new Token(TokenType.OperatorLessThan, "less than", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordRaised && i + 4 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordTo &&
                        tokens[i + 2].Type == TokenType.KeywordThe &&
                        tokens[i + 3].Type == TokenType.KeywordPower &&
                        tokens[i + 4].Type == TokenType.KeywordOf)
                    {
                        condensed.Add(new Token(TokenType.OperatorMathPow, "raised to the power of", token.Line, token.Column));
                        i += 4; // Skip the next four tokens as they have been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordOf && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordSize)
                    {
                        condensed.Add(new Token(TokenType.KeywordOfSize, "of size", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordOtherwise && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordIf)
                    {
                        condensed.Add(new Token(TokenType.KeywordOtherwiseIf, "otherwise if", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordPointer && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordTo)
                    {
                        condensed.Add(new Token(TokenType.KeywordPointerTo, "pointer to", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordFor && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordEach)
                    {
                        condensed.Add(new Token(TokenType.KeywordForEach, "for each", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordIterate && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordOver)
                    {
                        condensed.Add(new Token(TokenType.KeywordIterateOver, "iterate over", token.Line, token.Column));
                        i++; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordWith && i + 2 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordIndex &&
                        tokens[i + 2].Type == TokenType.KeywordAs)
                    {
                        condensed.Add(new Token(TokenType.KeywordWithIndexAs, "with index as", token.Line, token.Column));
                        i += 2; // Skip the next token as it has been combined.
                        continue;
                    }
                    else if (tokens[i + 1].Type == TokenType.KeywordValue &&
                        tokens[i + 2].Type == TokenType.KeywordAs)
                    {
                        condensed.Add(new Token(TokenType.KeywordWithValueAs, "with value as", token.Line, token.Column));
                        i += 2; // Skip the next token as it has been combined.
                        continue;
                    }
                }
                else if (token.Type == TokenType.KeywordDefine && i + 1 < tokens.Count)
                {
                    if (tokens[i + 1].Type == TokenType.KeywordFunction)
                    {
                        condensed.Add(new(TokenType.KeywordDefineFunction, token.Raw + " " + tokens[i + 1].Raw, token.Line, token.Column));
                        i++;
                        continue;
                    }
                    else if (tokens[i + 1].Type == TokenType.KeywordStructure)
                    {
                        condensed.Add(new(TokenType.KeywordDefineStructure, token.Raw + " " + tokens[i + 1].Raw, token.Line, token.Column));
                        i++;
                        continue;
                    }
                    else if (tokens[i + 1].Type == TokenType.TypeEnum) {
                        condensed.Add(new(TokenType.KeywordDefineEnum, token.Raw + " " + tokens[i + 1].Raw, token.Line, token.Column));
                        i++;
                        continue;
                    }
                }
                condensed.Add(token); // Add the token as is if no condensing occurred. 
            }
            return condensed;
        }

        private List<Token> CondenseNumbers(List<Token> tokens)
        {
            var condensed = new List<Token>();
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token.Type == TokenType.LiteralNumber || token.Type == TokenType.LiteralPosition)
                {
                    int endOfNumber = i;
                    while (endOfNumber + 1 < tokens.Count &&
                           (tokens[endOfNumber + 1].Type == TokenType.LiteralNumber ||
                            tokens[endOfNumber + 1].Type == TokenType.LiteralPosition))
                    {
                        endOfNumber++;
                    }
                    // Combine all number tokens into one.
                    var numberTokens = tokens[i..(endOfNumber + 1)];
                    if (TokenMatcher.TryConvertToDecimal(numberTokens.ToArray(), out var decimalValue))
                        condensed.Add(new Token(TokenType.LiteralNumber, decimalValue.ToString(), token.Line, token.Column));
                    else
                        // If conversion fails, keep the original tokens. // this should leave binary and hex in original state
                        condensed.AddRange(tokens[i..(endOfNumber + 1)]);
                    i = endOfNumber; // Move the index to the end of the number sequence.
                }
                else
                {
                    condensed.Add(token);
                }
            }
            return condensed;
        }
    }
}
