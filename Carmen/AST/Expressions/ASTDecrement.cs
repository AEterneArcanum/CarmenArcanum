using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Expressions;

public record ASTDecrement(
    ASTPosition Position, 
    ASTExpression Expression, 
    bool IsPrefix) 
    : ASTExpression(Position), 
        IStandalone, 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Expression];
    public override string ToString() => IsPrefix ? $"--{Expression}" : $"{Expression}--";
}
