namespace Arcane.Carmen.AST;
public record ASTNode(Position Position);
public record ASTStatement(Position Position) : ASTNode(Position), IStandalone;
public record ASTExpression(Position Position) : ASTNode(Position);