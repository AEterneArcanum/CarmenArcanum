using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ASTArrayStride(
        ASTPosition Position,
        ASTExpression Stride,
        ASTExpression Object)
        : ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children => [Stride, Object];
    }
}
