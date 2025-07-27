using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Literals;

public record ASTLitNumber(ASTPosition Position, decimal Value) : ASTExpression(Position)
{
    public override string ToString()
    {
        return Value.ToString();
    }
}
