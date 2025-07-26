namespace Arcane.Carmen.AST;
public record ASTIdentifier(Position Position, string Identifier) : ASTExpression(Position);