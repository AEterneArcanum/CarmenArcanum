
namespace Arcane.Carmen.AST;

public record ASTBlock(Position Position, ASTNode[] InnerNodes) : ASTStatement(Position);
