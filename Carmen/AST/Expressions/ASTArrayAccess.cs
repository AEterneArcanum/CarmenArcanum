using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Expressions;

public record ASTArrayAccess(ASTPosition Position, ASTExpression Object, ASTExpression Index) : ASTExpression(Position), IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Object, Index];

    public override string ToString()
    {
        return $"{Object}[{Index}]";
    }
}
