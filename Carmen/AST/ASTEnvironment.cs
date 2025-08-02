using Arcane.Carmen.AST.Statements;
using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST
{
    public record ASTEnvironment(
        ASTArchZone[] DefinitionZones,
        ASTEntryPoint EntryPoint)
    {
    }

    public record ASTArchZone(
        ASTArchitecture Architecture,
        ASTStructure[] Structures,
        ASTFunction[] Functions,
        ASTVariableDefinition[] Variables);
}
