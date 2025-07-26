using Arcane.Carmen.AST;
namespace Arcane.Carmen.Writer;
public interface IWriter
{
    public void Write(ASTNode[] nodes, string filename);
    public bool Validate(ASTNode[] nodes);
}
