

using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST;

public record ASTParenthized(
    ASTPosition Position, 
    ASTExpression InnerExpr) 
    : ASTExpression(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [InnerExpr];
}
