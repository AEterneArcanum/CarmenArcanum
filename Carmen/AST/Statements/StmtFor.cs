using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    /// <summary>
    /// 'for' VARIABLE_ID 'from' EXPRESSION 'to' EXPRESSION ('step' EXPRESSION)? 'do' STATEMENT
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="From"></param>
    /// <param name="To"></param>
    /// <param name="Step"></param>
    /// <param name="Body"></param>
    public record StmtFor(
        ExprIdentifier Id, 
        Expression From, 
        Expression To,
        Expression? Step, 
        Statement Body) : Statement;

    public class StmtForParser : StatementParser
    {
        public StmtForParser(int priority = StatementPriorities.Loop) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = null;
            if (tokens.Length < 8 || tokens[0].Type != TokenType.KeywordFor) 
                return false;
            // Find from
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordFrom, out int idxFrom))
                return false;
            // Find to
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordTo, out int idxTo)) return false;
            // find do
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordDo, out int idxDo)) return false;
            // Check for step
            bool hasStep = tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordStep, out int idxStep);
            
            // Id between for and from
            if (!Expression.TryParse(tokens[1..idxFrom], out var exprId) || 
                exprId is not ExprIdentifier || 
                ((ExprIdentifier)exprId).Type == IdentifierType.Variable)
                return false;
            // From between from and to
            if (!Expression.TryParse(tokens[(idxFrom + 1)..idxTo], out var exprFrom)) return false;
            // To between to and step/do
            if (!Expression.TryParse(tokens[(idxTo + 1)..(hasStep ? idxStep : idxDo)], out var exprTo)) return false;
            // Statements after do
            if (!Statement.TryParse(tokens[(idxDo + 1)..], out var stmtBody)) return false;

            // Exp Step between step and do
            if (hasStep)
            {
                if (!Expression.TryParse(tokens[(idxStep + 1)..idxDo], out var exprStep)) return false;
                result = new StmtFor((ExprIdentifier)exprId!, exprFrom!, exprTo!, exprStep, stmtBody!);
            }
            result = new StmtFor((ExprIdentifier)exprId!, exprFrom!, exprTo!, null, stmtBody!);
            return true;
        }
    }
}
