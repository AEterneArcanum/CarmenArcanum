using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;
namespace Arcane.Carmen.AST.Statements;

public enum ASTVariableType
{
    Generic,
    Static,
    Constant
}

public static class ASTVariableTypeEx
{
    public static string ToString(this ASTVariableType variableType)
    {
        return variableType switch
        {
            ASTVariableType.Static => "static ",
            ASTVariableType.Constant => "const ",
            _ => ""
        };
    }
}

public record ASTVariableDefinition(
    ASTPosition Position, 
    ASTIdentifier Identifier, 
    ASTVariableType VariableType,
    ASTTypeInfo Type,
    ASTExpression? Initial) 
    : ASTStatement(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Identifier];
    public override string ToString() => $"{VariableType.ToString()} {Type} {Identifier}{(Initial is null? "" : $" = {Initial}")}";
}
