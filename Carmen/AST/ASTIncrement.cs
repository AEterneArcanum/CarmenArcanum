using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST
{
    public record ASTIncrement(
        ASTPosition Position, 
        ASTExpression Expression, 
        bool IsPrefix) 
        : ASTExpression(Position), 
            IStandalone, 
            IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children => [Expression];
    }
}
