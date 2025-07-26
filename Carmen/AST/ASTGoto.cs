namespace Arcane.Carmen.AST;
public record ASTGoto(Position Position, ASTIdentifier Identifier) : ASTStatement(Position);
