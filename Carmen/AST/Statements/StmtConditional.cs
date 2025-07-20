using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Statements
{
    public record ElseCondition(Expression Condition, Statement Execution);

    public record StmtConditional(
        Expression Condition, 
        Statement Execution, 
        ElseCondition[] Elifs, 
        Statement? Else)
        : Statement
    {
    }

    public class StmtConditionalParser : StatementParser
    {
        public StmtConditionalParser(int priority = StatementPriorities.Conditional) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordIf)
                return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordThen, out var idxThen))
                return false;
            // Check fo elifs at top level
            var elifs = tokens.TopLayerIndicesOf(TokenType.KeywordOtherwiseIf);
            var hasElse = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordOtherwise, out var idxOtherwise);
            // ensure elifs are after then and before else
            if (elifs[0] < idxThen || elifs[^1] > idxOtherwise)
                return false;
            if (!hasElse && elifs.Count == 0) // No alt paths
            {
                if (!Expression.TryParse(tokens[1..idxThen], out var expression) ||
                    !Statement.TryParse(tokens[(idxThen + 1)..], out var statement)) return false;
                result = new StmtConditional(expression!, statement!, [], null);
                return true;
            }
            else if (elifs.Count == 0) // else but no elifs
            {
                if (!Expression.TryParse(tokens[1..idxThen], out var expression) ||
                    !Statement.TryParse(tokens[(idxThen + 1)..idxOtherwise], out var state1) ||
                    !Statement.TryParse(tokens[(idxOtherwise + 1)..], out var stateElse))
                    return false;
                result = new StmtConditional(expression!, state1!, [], stateElse!);
                return true;
            }
            else if (!hasElse) // elifs but no else
            {
                if (!Expression.TryParse(tokens[1..idxThen], out var condition) ||
                    !Statement.TryParse(tokens[(idxThen + 1)..elifs[0]], out var statement))
                    return false;
                var elseifs = new List<ElseCondition>();

                for (int i = 0; i < elifs.Count - 1; i++)
                {
                    var elif = elifs[i];
                    // Get index of next then
                    if (!tokens[elif..elifs[i + 1]].Any(x => x.Type == TokenType.KeywordThen))
                        return false; // then not in range
                    if (!tokens.TryGetFirstTopLayerIndexOfFrom(TokenType.KeywordThen, elif, out var idxSubThen))
                        return false; // then not found for this
                    // Get expression after elif to then
                    if (!Expression.TryParse(tokens[(elif + 1)..idxSubThen], out var subCondition))
                        return false;
                    // Get statements from then to next elif
                    if (!Statement.TryParse(tokens[(idxSubThen + 1)..(elifs[i + 1])], out var subStatement))
                        return false;
                    elseifs.Add(new ElseCondition(condition!, statement!));
                }
                // Add final elif there is no then statement
                if (!tokens[(elifs[^1] + 1)..].Any(x => x.Type == TokenType.KeywordThen))
                    return false;
                if (!tokens.TryGetFirstTopLayerIndexOfFrom(TokenType.KeywordThen, elifs[^1], out var finThen))
                    return false;
                if (!Expression.TryParse(tokens[(elifs[^1] + 1)..finThen], out var finCondition))
                    return false;
                if (!Statement.TryParse(tokens[(finThen + 1)..],  out var finStatement))
                    return false;
                elseifs.Add(new ElseCondition(finCondition!, finStatement!));

                result = new StmtConditional(condition!, statement!, [..elseifs], null);
                return true;
            }
            else // has elifs and else
            {
                // Condition
                if (!Expression.TryParse(tokens[1..idxThen], out var condition) ||
                    !Statement.TryParse(tokens[(idxThen + 1)..elifs[0]], out var statement) ||
                    !Statement.TryParse(tokens[(idxOtherwise + 1)..], out var stateElse))
                    return false;
                // build elifs
                var elseifs = new List<ElseCondition>();
                for (int i = 0; i < elifs.Count - 1; i++)
                {
                    var elif = elifs[i];
                    // Get index of next then
                    if (!tokens[elif..elifs[i + 1]].Any(x => x.Type == TokenType.KeywordThen))
                        return false; // then not in range
                    if (!tokens.TryGetFirstTopLayerIndexOfFrom(TokenType.KeywordThen, elif, out var idxSubThen))
                        return false; // then not found for this
                    // Get expression after elif to then
                    if (!Expression.TryParse(tokens[(elif + 1)..idxSubThen], out var subCondition))
                        return false;
                    // Get statements from then to next elif
                    if (!Statement.TryParse(tokens[(idxSubThen + 1)..(elifs[i + 1])], out var subStatement))
                        return false;
                    elseifs.Add(new ElseCondition(condition!, statement!));
                }
                // Get Final else if
                if (!tokens[(elifs[^1] + 1)..idxOtherwise].Any(x => x.Type == TokenType.KeywordThen))
                    return false;
                if (!tokens.TryGetFirstTopLayerIndexOfFrom(TokenType.KeywordThen, elifs[^1], out var finThen))
                    return false;
                if (!Expression.TryParse(tokens[(elifs[^1] + 1)..finThen], out var finCondition))
                    return false;
                if (!Statement.TryParse(tokens[(finThen + 1)..idxOtherwise], out var finStatement))
                    return false;
                elseifs.Add(new ElseCondition(finCondition!, finStatement!));

                result = new StmtConditional(condition!, statement!, [..elseifs], stateElse);
                return true;
            }
        }
    }
}
