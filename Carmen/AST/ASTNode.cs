using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST;
public record ASTNode(ASTPosition Position);
public record ASTStatement(ASTPosition Position) : ASTNode(Position), IStandalone;
public record ASTExpression(ASTPosition Position) : ASTNode(Position);