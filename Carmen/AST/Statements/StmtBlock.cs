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
            if (tokens.Length < 2 || tokens[0].Type != TokenType.BlockStart || tokens[^1].Type != TokenType.BlockEnd)
            {
                return false;
            }
            var statements = new List<Statement>();
            var innerTokens = tokens[1..^1];

            int index = 0; // get a single command or sub-block // items with blocks dont have an eos before the block
            while (index < innerTokens.Length) {
                // Start from index and build a complete statement be finding the next eos or block close
                int traverser = index;
                while ((innerTokens[traverser].Type != TokenType.EndOfStatement
                    && innerTokens[traverser].Type != TokenType.BlockEnd) && (traverser + 1 < tokens.Length)) traverser++;
                var statmentTokens = innerTokens[index .. (traverser + 1)]; // get the tokens for the statement include the eos or block end
                if (!Statement.TryParse(statmentTokens, out var localStatement)) return false;
                statements.Add(localStatement!);
                index = traverser + 1; // move to the next statement, skipping the eos or block end
            }

            result = new StmtBlock(statements.ToArray());
            return true;
        }
    }
}
