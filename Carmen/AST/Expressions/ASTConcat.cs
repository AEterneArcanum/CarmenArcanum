using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ASTConcat(ASTPosition Position,
        ASTExpression Left,
        ASTExpression Right)
        : ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children => [Left, Right];
    }
}
