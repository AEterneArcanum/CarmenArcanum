using Arcane.Carmen.AST.Types;
namespace Arcane.Carmen.AST.Literals;
public record ASTLitChar(ASTPosition Position, string Value) : ASTExpression(Position)
{
    public override string ToString()
    {
        return Value;
    }
}
