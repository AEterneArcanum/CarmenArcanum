using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Statements;

public record ASTArchDefined(
    ASTPosition Position,
    ASTArchitecture Architecture,
    ASTStatement Statement)
    : ASTStatement(Position), IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Statement];
}
