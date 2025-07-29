using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Literals;

public record ASTLitBool(ASTPosition Position, bool Value) : ASTExpression(Position)
{
    public override string ToString()
    {
        return Value ? "true" : "false";
    }
}
