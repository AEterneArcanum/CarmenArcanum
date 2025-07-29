using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Expressions;

public enum ASTBitwiseOp 
{
    ShiftLeft, ShiftRight,
    RotateLeft, RotateRight,

    AND,
    OR,
    XOR
}

public record ASTBitwise(
    ASTPosition Position,
    ASTBitwiseOp Operation,
    ASTExpression Left,
    ASTExpression Right)
    : ASTExpression(Position), IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => 
        [Left, Right];
}
