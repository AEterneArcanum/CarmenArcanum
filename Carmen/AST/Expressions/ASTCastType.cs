using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Expressions
{
    public record ASTCastType(
        ASTPosition Position,
        ASTExpression Object,
        ASTTypeInfo CastType,
        bool SafeCast)
        : ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children 
            => [Object];
    }
}
