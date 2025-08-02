using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public enum ASTCompoundOp
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        Concatenation,
    }

    public record ASTCompoundAssign(
        ASTPosition Position,
        ASTExpression Object,
        ASTCompoundOp Operation,
        ASTExpression Value)
        : ASTStatement(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children 
            => [Object, Value];
    }
}
