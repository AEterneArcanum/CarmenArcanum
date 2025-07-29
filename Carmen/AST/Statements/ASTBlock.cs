using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Statements;

public record ASTBlock(
    ASTPosition Position, 
    ASTNode[] InnerNodes) 
    : ASTStatement(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => InnerNodes;
    public override string ToString()
    {
        string r = "{\n";
        foreach (ASTNode node in Children)
        {
            r += node.ToString();
            r += ";\n";
        }
        r += "}\n";
        return r;
    }
}
