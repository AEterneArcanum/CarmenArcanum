using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Statements;
public record ASTGoto(
    ASTPosition Position, 
    ASTIdentifier Identifier) 
    : ASTStatement(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Identifier];
    public override string ToString() => $"goto {Identifier}";
}
