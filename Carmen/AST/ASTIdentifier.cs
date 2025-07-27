using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST;
public record ASTIdentifier(ASTPosition Position, string Identifier) : ASTExpression(Position);