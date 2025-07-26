namespace Arcane.Carmen.AST;
/// <summary>
/// Represents the programs main entry point in the code.
/// </summary>
public record ASTEntryPoint(Position Position, ASTNode Code) : ASTStatement(Position);
