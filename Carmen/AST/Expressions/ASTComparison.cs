using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions;

public enum ASTComparisonOp
{
    Equal,
    NotEqual,
    LessThan, LessThanOrEqual,
    GreaterThan, GreaterThanOrEqual,
}

public static class ASTComparisonOpEx
{
    public static string ToString(this ASTComparisonOp op)
    {
        return op switch
        {
            ASTComparisonOp.Equal => "==",
            ASTComparisonOp.NotEqual => "!=",
            ASTComparisonOp.LessThan => "<",
            ASTComparisonOp.GreaterThan => ">",
            ASTComparisonOp.LessThanOrEqual => "<=",
            ASTComparisonOp.GreaterThanOrEqual => ">=",
            _ => "??????"
        };
    }
}

public record ASTComparison(
    ASTPosition Position, 
    ASTComparisonOp Op, 
    ASTExpression Left, 
    ASTExpression Right) 
    : ASTExpression(Position), 
        IHasInnerNodes
{
    public IEnumerable<ASTNode> Children => [Left, Right];
    public override string ToString() => $"{Left} {Op.ToString()} {Right}";
}
