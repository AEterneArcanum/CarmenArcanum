using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST;
public record ASTVariableDefinition(
    ASTPosition Position, 
    ASTIdentifier Identifier, 
    ASTTypeInfo Type) 
    : ASTStatement(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Identifier];
}
