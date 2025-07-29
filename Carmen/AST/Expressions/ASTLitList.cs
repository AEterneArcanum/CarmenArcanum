using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ASTLitList(ASTPosition Position, ASTExpression[] ListItems) : ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children => ListItems;
        public override string ToString()
        {
            string r = "[";
            foreach (ASTNode item in Children)
            {
                r += item.ToString();
                r += ", ";
            }
            r = r.Trim().TrimEnd(',');
            r += "]";
            return r;
        }
    }
}
