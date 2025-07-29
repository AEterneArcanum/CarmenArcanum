using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Statements;
public record ASTAssignment(
    ASTPosition Position, 
    ASTExpression Object, 
    ASTExpression Value) 
    : ASTStatement(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Object, Value];
    public override string ToString()
    {
        return $"{Object} = {Value}";
    }
}