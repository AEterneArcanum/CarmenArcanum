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
public record ASTComparison(Position Position, ASTComparisonOp Op, ASTExpression Left, ASTExpression Right):ASTExpression(Position);
