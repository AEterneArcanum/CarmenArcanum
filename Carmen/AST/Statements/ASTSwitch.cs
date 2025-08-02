using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Statements
{
    public record ASTSwitch(
        ASTPosition Position,
        ASTExpression Expression,
        ASTBlock Body)
        : ASTStatement(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children => [Expression, Body];
    }
}
