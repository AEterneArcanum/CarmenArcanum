using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Literals;
public record ASTIdentifier(ASTPosition Position, string Identifier) : ASTExpression(Position)
{
    public override string ToString()
    {
        return Identifier;
    }
}