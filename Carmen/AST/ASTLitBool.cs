namespace Arcane.Carmen.AST;

public record ASTLitBool(Position Position, bool Value) : ASTExpression(Position);
