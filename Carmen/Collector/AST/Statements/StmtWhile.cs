using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST.Statements
{
    /// <summary>
    /// 'while' EXPRESSION 'do' STATEMENT
    /// </summary>
    /// <param name="Expression">Conditional expression.</param>
    /// <param name="Body">Statement body.</param>
    public record StmtWhile(Expression Expression, Statement Body) : Statement;

    public class StmtWhileParser : StatementParser
    {
        public StmtWhileParser() : base(StatementPriorities.Loop) { }

        public StmtWhileParser(int priority = StatementPriorities.Loop) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type == TokenType.KeywordWhile) return false;
            // Find do
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordDo, out var idxDo))
                return false;
            if (!Expression.TryParse(tokens[1..idxDo], out var expression)) return false;
            if (!Statement.TryParse(tokens[(idxDo + 1)..], out var body)) return false;
            result = new StmtWhile(expression!, body!);
            return true;
        }
    }
}
