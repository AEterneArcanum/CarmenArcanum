using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Literals
{
    public record ASTLitNull(ASTPosition Position) : ASTExpression(Position)
    {
        public override string ToString()
        {
            return "NULL";
        }
    }
}
