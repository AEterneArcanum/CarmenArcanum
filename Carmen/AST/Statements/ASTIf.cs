using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Statements;

public record ASTIf(
    ASTPosition Position, 
    ASTExpression Condition, 
    ASTNode Body) 
    : ASTStatement(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Condition, Body];
    public override string ToString() => $"if ({Condition}) {Body};";
}
