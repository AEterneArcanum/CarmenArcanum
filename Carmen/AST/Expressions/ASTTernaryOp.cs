using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ASTTernaryOp(
        ASTPosition Position,
        ASTExpression Condition,
        ASTExpression IfTrue,
        ASTExpression IfFalse)
        : ASTExpression(Position), IHasInnerNodes
    {
        IEnumerable<ASTNode> IHasInnerNodes.Children 
            => [Condition, IfTrue, IfFalse];
    }
}
