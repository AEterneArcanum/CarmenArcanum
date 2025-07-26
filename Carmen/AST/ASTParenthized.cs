
namespace Arcane.Carmen.AST;

public record ASTParenthized(Position Position, ASTExpression InnerExpr) : ASTExpression(Position);
