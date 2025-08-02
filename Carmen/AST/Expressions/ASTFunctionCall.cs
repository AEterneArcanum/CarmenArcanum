using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ASTFunctionCall(
        ASTPosition Position,
        ASTIdentifier Identifier,
        ASTExpression[] Parameters)
        : ASTExpression(Position), IHasInnerNodes, IStandalone
    {
        public IEnumerable<ASTNode> Children 
            => [Identifier, ..Parameters];
    }
}
