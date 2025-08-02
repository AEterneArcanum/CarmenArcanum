using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;

namespace Arcane.Carmen.AST.Statements
{
    public record ASTEnumDefinition(
        ASTPosition Position,
        ASTIdentifier Iden,
        ASTIdentifier[] Elements)
        : ASTStatement(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children
        {
            get
            {
                var x = new List<ASTNode>
                {
                    Iden
                };
                x.AddRange(Elements);
                return x;
            }
        }
    }
}
