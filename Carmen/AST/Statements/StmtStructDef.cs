using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Statements
{
    /// <summary>
    /// STRUCTURE_DEFINITION --> 'define structure' STRUCTURE_ID 'with' ARRAYLITERAL # <-- Of VARIABLE_DEFINITION 
    /// # '; VARIABLE_DEFINITION, VARIABLE_DEFINITION, VARIABLE_DEFINITION, and VARIABLE_DEFINITION.'
    /// # Ended with EOS
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="Definition"></param>
    public record StmtStructDef(ExprIdentifier Id, ExprStructParameters Definition) : Statement;

    public class StmtStructDefParser : StatementParser
    {
        public StmtStructDefParser(int priority = StatementPriorities.Definitions) : base(priority)
        {
        }

        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            result = default;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordDefineStructure)
                return false;
            // Find with
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordWith, out var idxWith))
                return false;
            // Parse id
            if (!Expression.TryParse(tokens[1..idxWith], out var idExpression) ||
                idExpression is not ExprIdentifier || ((ExprIdentifier)idExpression).Type != IdentifierType.Structure)
                return false;
            // Parse definition
            if (!Expression.TryParse(tokens[(idxWith + 1)..], out var defExpression) ||
                defExpression is not ExprStructParameters)
                return false;

            result = new StmtStructDef((ExprIdentifier)idExpression!, (ExprStructParameters)defExpression!);
            return true;
        }
    }
}
