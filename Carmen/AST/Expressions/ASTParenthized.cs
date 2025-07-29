using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Expressions;

public record ASTParenthized(
    ASTPosition Position, 
    ASTExpression InnerExpr) 
    : ASTExpression(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [InnerExpr];
    public override string ToString() => $"({InnerExpr})";
}
