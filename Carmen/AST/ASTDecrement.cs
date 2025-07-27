using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST;

public record ASTDecrement(
    ASTPosition Position, 
    ASTExpression Expression, 
    bool IsPrefix) 
    : ASTExpression(Position), 
        IStandalone, 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Expression];
}
