using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtImport(Expression Imported, Expression? Alias) : Statement
    {
        public override string ToString()
        {
            return Alias is null ? $"import {Imported};" : $"import {Imported} as {Alias};";
        }
    }
    public class StmtImportParser : StatementParser
    {
        public StmtImportParser(int priority = StatementPriorities.Import) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 3 || tokens[0].Type != TokenType.KeywordImport)
                return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordAs, out int index))
            {
                // no alias
                if (!Expression.TryParse(tokens[1..], out var imported))
                    return false;
                result = new StmtImport(imported!, null);
                return true;
            }
            else
            {
                // with alias
                if (index < 2 || index >= tokens.Length - 1)
                    return false;
                if (!Expression.TryParse(tokens[1..index], out var imported) ||
                    !Expression.TryParse(tokens[(index + 1)..], out var alias))
                {
                    return false;
                }
                result = new StmtImport(imported!, alias!);
                return true;
            }
        }
    }
}
