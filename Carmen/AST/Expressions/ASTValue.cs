using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Expressions
{
    public record ASTValue(ASTPosition Position) : ASTExpression(Position)
    {
    }
}
