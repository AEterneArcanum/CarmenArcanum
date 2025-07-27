using Arcane.Carmen.AST;
namespace Arcane.Carmen.Writer;
public interface IWriter
{
    public void WriteNodes(IEnumerable<ASTNode> nodes);
    public bool ValidateNodes(IEnumerable<ASTNode> nodes);
    public void SaveFile(string filename);
    public void Clear();
}
