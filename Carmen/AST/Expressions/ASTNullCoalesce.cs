using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ASTNullCoalesce(
        ASTPosition Position,
        ASTExpression Value,
        ASTExpression Alternate)
        : ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children 
            => [Value, Alternate];
    }
}
