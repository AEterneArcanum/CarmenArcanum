using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Statements;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public record ExprStructParameters(StmtVarDef[] Members) : Expression;

    public class ExprStructParametersParser : ExpressionParser
    {
        public ExprStructParametersParser() : base(ExpressionPriorities.ListExpression) { }
        public ExprStructParametersParser(int priority = ExpressionPriorities.ListExpression) : base(priority) { }

        public override bool TryParse(Token[] tokens, out Expression? result)
        {
            result = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.PunctuationSemicolon)
                return false;
            List<StmtVarDef> vars = new List<StmtVarDef>();

            List<int> commas = tokens.TopLayerIndicesOfXBeforeY(TokenType.PunctuationComma, TokenType.KeywordCommaAnd);
            commas.Add(tokens.FirstTopLayerIndexOf(TokenType.KeywordCommaAnd)); // Add the index of the ", and" keyword if it exists.

            int expressionCount = commas.Count + 1;

            if (expressionCount == 1)
            {
                var expressionTokens = tokens[1..];
                if (!Statement.TryParse(expressionTokens, out var statement) ||
                    statement is not StmtVarDef) return false;
                vars.Add((StmtVarDef)statement);
            }
            else
            {
                int sti = 1;
                for (int i = 0; i < commas.Count; i++)
                {
                    int end = commas[i];
                    var statementTokens = tokens[sti..end];
                    if (!Statement.TryParse(statementTokens, out var statement) ||
                        statement is not StmtVarDef) return false;
                    vars.Add((StmtVarDef)statement);
                    sti = end + 1;
                }
                var lastTokens = tokens[sti..];
                if (!Statement.TryParse(lastTokens, out var lastStatement) ||
                    lastStatement is not StmtVarDef) return false;
                vars.Add((StmtVarDef)lastStatement);
            }

            result = new ExprStructParameters([.. vars]);
            return true;
        }
    }
}
