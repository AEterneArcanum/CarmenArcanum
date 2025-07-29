using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public enum ASTMathOp
    {
        Add,
        Subtract,
        Multiply,
        Divide,

        Modulo,

        Power,
        Root
    }

    public record ASTMath(
        ASTPosition Position, 
        ASTMathOp Op, 
        ASTExpression Left, 
        ASTExpression Right) 
        : ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children => [Left, Right];
    }
}
