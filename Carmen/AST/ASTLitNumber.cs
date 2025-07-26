namespace Arcane.Carmen.AST;

public record ASTLitNumber(Position Position, Int128 Value) : ASTExpression(Position);
