using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.Lexer.Tokens
{
    public interface ISymbolMatcher
    {
        /// <summary>
        /// Escape char in string literals.
        /// </summary>
        public char StringEscapeChar { get; }
        /// <summary>
        /// Escape char in charachter literals.
        /// </summary>
        public char CharEscapeChar { get; }
        /// <summary>
        /// Because.
        /// </summary>
        public char NewLineChar { get; }
        /// <summary>
        /// Outer char on each side of a multiline comment.
        /// </summary>
        public char MultilineCommentOuterChar { get; }
        /// <summary>
        /// Inner char on each side of a multiline comment.
        /// </summary>
        public char MultilineCommentInnerChar { get; }
        /// <summary>
        /// First char marking a single line comment.
        /// </summary>
        public char CommentOuterChar { get; }
        /// <summary>
        /// Second char marking a single line comment.
        /// </summary>
        public char CommentInnerChar { get; }
        /// <summary>
        /// Character surrounding strings.
        /// </summary>
        public char StringLiteralChar { get; }
        /// <summary>
        /// Character to surround chars.
        /// </summary>
        public char CharLiteralChar { get; }
        /// <summary>
        /// Minus sign for number processing.
        /// </summary>
        public char NegativeChar { get; }
        /// <summary>
        /// Decimal point for numbers
        /// </summary>
        public char DecimalChar { get; }
        /// <summary>
        /// For hex and binary prefix
        /// </summary>
        public char CharZero {  get; } 
        /// <summary>
        /// prefix hex numbers
        /// </summary>
        public char HexPrefix { get; }
        /// <summary>
        /// For binary prefix
        /// </summary>
        public char BinaryPrefix { get; }
        /// <summary>
        /// Is char a digit
        /// </summary>
        /// <returns></returns>
        public bool IsDigit(char c);
        /// <summary>
        /// Is char hex digit
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool IsHexDigit(char c);
        /// <summary>
        /// Is char a binary digit
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool IsBinaryDigit(char c);
        /// <summary>
        /// Char is whitespace
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool IsWhiteSpace(char c);
        /// <summary>
        /// Is it punctual
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool IsPunctuation(char c);
    }
}
