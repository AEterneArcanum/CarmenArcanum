using Arcane.Carmen.AST.Types;
namespace Arcane.Carmen.AST;
public record ASTVariableDefinition(Position Position, ASTIdentifier Identifier, ASTTypeInfo Type) : ASTStatement(Position); 
