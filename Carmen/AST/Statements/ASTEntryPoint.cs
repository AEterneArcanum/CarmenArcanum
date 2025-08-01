﻿using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Statements;
/// <summary>
/// Represents the programs main entry point in the code.
/// </summary>
public record ASTEntryPoint(
    ASTPosition Position, 
    ASTNode Code) 
    : ASTStatement(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Code];
    public override string ToString()
    {
        return $"program: {Code};";
    }
}
