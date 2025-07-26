namespace Arcane.Carmen.AST;
public record ASTAssignment(Position Position, ASTExpression Object, ASTExpression Value) : ASTStatement(Position);