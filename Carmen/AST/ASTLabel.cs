
using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST;

/// <summary>
/// Represents a label statement in the code.
/// </summary>
/// <param name="Position"></param>
/// <param name="Identifier"></param>
public record ASTLabel(
    ASTPosition Position, 
    ASTIdentifier Identifier) 
    : ASTStatement(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Identifier];
}
