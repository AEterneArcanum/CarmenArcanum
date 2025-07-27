using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST;

public enum ASTComparisonOp
{
    Equal,
    NotEqual,
    LessThan, LessThanOrEqual,
    GreaterThan, GreaterThanOrEqual,
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
}
