using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ASTArraySlice(
        ASTPosition Position,
        ASTExpression? Start,
        ASTExpression? End,
        ASTExpression Object)
        : ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children
        {
            get
            {
                var nodes = new List<ASTNode>();
                if (Start != null) nodes.Add(Start);
                if (End != null) nodes.Add(End);
                nodes.Add(Object);
                return nodes;
            }
        }
    }
}
