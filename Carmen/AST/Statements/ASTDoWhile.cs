using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record ASTDoWhile(
        ASTPosition Position,
        ASTExpression Condition,
        ASTBlock Body,
        bool IsUntil,
        bool Prefix) // While vs Do While
        : ASTStatement(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children => [Condition, Body];
    }
}
