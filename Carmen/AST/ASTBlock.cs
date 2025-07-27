using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST;

public record ASTBlock(
    ASTPosition Position, 
    ASTNode[] InnerNodes) 
    : ASTStatement(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => InnerNodes;
}
