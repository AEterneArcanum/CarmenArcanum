using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record ASTCase(
        ASTPosition Position,
        ASTExpression Expression)
        : ASTStatement(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children => [Expression];
    }
}
