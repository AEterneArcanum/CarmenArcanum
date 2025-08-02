using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record ASTASM(
        ASTPosition Position,
        bool IsUnsafe,
        ASTArchitecture Architecture,
        ASTLitString? Clobbers,
        ASTLitString? Code)
        : ASTStatement(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children 
            => (Clobbers is null) ? ((Code is null)? new List<ASTNode>() : [Code])
            : ((Code is null) ? [Clobbers] : [Clobbers, Code]);
    }
}
