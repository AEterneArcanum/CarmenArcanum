
using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST;
public record ASTGoto(
    ASTPosition Position, 
    ASTIdentifier Identifier) 
    : ASTStatement(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Identifier];
}
