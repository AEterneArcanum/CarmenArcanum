using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtAssignment(Expression Identifier, Expression Value) : Statement
    {
        public override string ToString()
        {
            return $"{Identifier} = {Value};";
        }
    }
    public class StmtAssignmentParser : StatementParser
    {
        public StmtAssignmentParser(int priority = StatementPriorities.Assignment) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordSet)
                return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordEqualTo, out int index))
                return false;
            if (index < 2 || index >= tokens.Length - 1)
                return false;
            if (!Expression.TryParse(tokens[1..index], out var identifier) ||
                !Expression.TryParse(tokens[(index + 1)..], out var value))
            {
                return false;
            }
            result = new StmtAssignment(identifier!, value!);
            return true;
        }
    }
}
