using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public enum ASTBooleanOp
    {
        Or,
        And,  
        Xor,

        IsType,
        IsNotType
    }

    public static class ASTBooleanOpEx
    {
        public static string ToString(this ASTBooleanOp op)
        {
            return op switch
            {
                ASTBooleanOp.Or => "||",
                ASTBooleanOp.And => "&&",
                ASTBooleanOp.Xor => "^",
                _ => "??????",
            };
        }
    }

    public record ASTBoolean(ASTPosition Position, ASTBooleanOp Op, ASTExpression Left, ASTExpression Right) : ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children => [Left, Right];
    }
}
