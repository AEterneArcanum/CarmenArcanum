using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtAlias(Expression Identifier, Expression Value) : Statement
    {
        public override string ToString()
        {
            return $"alias {Identifier} = {Value};";
        }
    }
    public class StmtAliasParser : StatementParser
    {
        public StmtAliasParser(int priority = StatementPriorities.Alias) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordAlias)
                return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordAs, out int index))
                return false;
            if (index < 2 || index >= tokens.Length - 1)
                return false;
            if (!Expression.TryParse(tokens[1..index], out var identifier) ||
                !Expression.TryParse(tokens[(index + 1)..], out var value))
            {
                return false;
            }
            result = new StmtAlias(identifier!, value!);
            return true;
        }
    }
}
