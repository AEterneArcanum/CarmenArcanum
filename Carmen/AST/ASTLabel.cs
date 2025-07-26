namespace Arcane.Carmen.AST;

/// <summary>
/// Represents a label statement in the code.
/// </summary>
/// <param name="Position"></param>
/// <param name="Identifier"></param>
public record ASTLabel(Position Position, ASTIdentifier Identifier) : ASTStatement(Position);
