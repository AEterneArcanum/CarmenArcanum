using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Statements
{
    /// <summary>
    /// 'import' STRINGLITERAL 'as' ALIAS_ID # 
    /// </summary>
    /// <param name="Imported"></param>
    /// <param name="Alias"></param>
    public record StmtImport(ExprStringLiteral Imported, ExprIdentifier Alias) : Statement;

    public class StmtImportParser : StatementParser
    {
        public StmtImportParser() : base(StatementPriorities.Import) { }
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
                return false; // must alias
            }
            else
            {
                // with alias
                if (index < 2 || index >= tokens.Length - 1)
                    return false;
                if (!Expression.TryParse(tokens[1..index], out var imported) ||
                    imported is not ExprStringLiteral ||
                    !Expression.TryParse(tokens[(index + 1)..], out var alias) ||
                    alias is not ExprIdentifier || ((ExprIdentifier)alias).Type != IdentifierType.Alias)
                {
                    return false;
                }
                result = new StmtImport((ExprStringLiteral)imported!, (ExprIdentifier)alias!);
                return true;
            }
        }
    }
}
