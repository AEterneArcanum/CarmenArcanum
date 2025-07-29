using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Expressions;

public record ASTBooleanNot(
    ASTPosition Position, 
    ASTExpression InnerExpression) 
    : ASTExpression(Position), IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [InnerExpression];
}
