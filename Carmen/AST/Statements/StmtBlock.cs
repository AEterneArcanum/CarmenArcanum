using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtBlock(Statement[] Statements) : Statement
    {
    }

    public class StmtBlockParser : StatementParser
    {
        public StmtBlockParser(int priority = StatementPriorities.Block) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.BlockStart)
            {
                return false;
            }
            var statements = new List<Statement>();
            var innerTokens = tokens[1..];
            int index = 0;

            while (index < innerTokens.Length) {
                int end = FindStatementEnd(innerTokens, index);
                if (end < 0 || end >= innerTokens.Length)
                {
                    if (!TryParseRemainingTokens(innerTokens[index..], statements))
                        return false;
                    break;
                }

                bool isBlockEnd = innerTokens[end].Type == TokenType.BlockEnd;
                var statementTokens = innerTokens[index..(isBlockEnd ? 1 : 0)];

                if (!Statement.TryParse(statementTokens, out var statement))
                    return false;

                statements.Add(statement!);
                index = end + 1;

                if (isBlockEnd && IsTerminalBlockEnd(innerTokens, end))
                    break;
            }

            result = new StmtBlock(statements.ToArray());
            return true;
        }
        /// <summary>
        /// Check if the token at the given index is the final block end.
        /// </summary>
        /// <param name="tokens">Tokens.</param>
        /// <param name="index">Index of found end.</param>
        /// <returns>Is terminal block end.</returns>
        private bool IsTerminalBlockEnd(Token[] tokens, int index)
        {
            // Cannot be terminal block end except at this index.
            return index + 1 == tokens.Length;
        }
        /// <summary>
        /// Attempt to parse remaining provided tokens.
        /// </summary>
        /// <param name="remainingTokens">All leftover tokens.</param>
        /// <param name="statements">List to store statement of successful parse.</param>
        /// <returns>Parse success.</returns>
        private bool TryParseRemainingTokens(Token[] remainingTokens, List<Statement> statements)
        {
            if (remainingTokens.Length == 1 && remainingTokens[0].Type == TokenType.BlockEnd)
                return true;
            if (!Statement.TryParse(remainingTokens, out var finalStatement)) return false;
            statements.Add((Statement)finalStatement!);
            return true;
        }
        /// <summary>
        /// Starting at index 'start' find the next eos or closing block accounting for 'while' 'otherwise' and 'otherwise if' keywords immediately following the found terminator.
        /// </summary>
        /// <param name="tokens">Tokens to search.</param>
        /// <param name="start">Index to begin search at.</param>
        /// <returns>Index statement end.</returns>
        private int FindStatementEnd(Token[] tokens, int start)
        {
            int depth = 0; // Start and ground floor.
            bool inDoBlock = false;

            for (int i = start; i < tokens.Length; i++) {
                switch (tokens[i].Type) {
                    case TokenType.KeywordDo:
                        inDoBlock = true; 
                        break;
                    case TokenType.BlockStart:
                        depth++; break; // Entered a sub block.
                    case TokenType.BlockEnd:
                        depth--; // Exited a sub block.
                        if (depth == 0) // On ground floor.
                        {
                            // Trailing Keywords
                            if (i + 1 < tokens.Length)
                            {
                                var next = tokens[i + 1].Type;

                                // DoWhile Block Exit
                                if (inDoBlock && next == TokenType.KeywordWhile)
                                {
                                    inDoBlock = false;
                                    continue;
                                }

                                // Chained conditionals
                                if (next is TokenType.KeywordOtherwiseIf or TokenType.KeywordOtherwise)
                                    continue;
                            }
                            return i; // Final token of tokens.
                        }
                        break; // Exited a sub block.
                    case TokenType.EndOfStatement:
                        if (depth == 0 && !inDoBlock) return i; // End of statement on ground floor.
                        break;
                }
            }
            return -1; // No Token found.
        }
    }
}
