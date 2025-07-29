using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Expressions;

public record ASTAddressOf(ASTPosition Position, ASTExpression Expression) : ASTExpression(Position), IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Expression];
    public override string ToString()
    {
        return $"&{Expression}";
    }
}
