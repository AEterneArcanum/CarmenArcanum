using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.Lexer.Tokens.Matchers
{
    public class EnglishSymbolMatcher
        : ISymbolMatcher
    {
        public char StringEscapeChar => '\\';

        public char CharEscapeChar => '\\';

        public char NewLineChar => '\n';

        public char MultilineCommentOuterChar => '\\';

        public char MultilineCommentInnerChar => '*';

        public char CommentOuterChar => '\\';

        public char CommentInnerChar => '\\';

        public char StringLiteralChar => '\"';

        public char CharLiteralChar => '\'';

        public char NegativeChar => '-';

        public char DecimalChar => '.';

        public char CharZero => '0';

        public char HexPrefix => 'x';

        public char BinaryPrefix => 'b';

        public bool IsBinaryDigit(char c)
        {
            return c == '0' || c == '1';
        }

        public bool IsDigit(char c) => char.IsDigit(c);

        public bool IsHexDigit(char c) => char.IsAsciiHexDigit(c);

        public bool IsWhiteSpace(char c) => char.IsWhiteSpace(c);

        public bool IsPunctuation(char c) => char.IsPunctuation(c) && c != '_';
    }
}
