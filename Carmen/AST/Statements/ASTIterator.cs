using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record ASTIterator(
        ASTPosition Position,
        ASTExpression Object,
        ASTBlock Body)
        : ASTStatement(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children => [Object, Body];
    }
}
