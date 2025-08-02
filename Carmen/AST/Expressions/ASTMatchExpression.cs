using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ASTMatch(ASTPosition Position, ASTExpression In, ASTExpression Out);

    public record ASTMatchExpression(
        ASTPosition Position,
        ASTExpression Object,
        ASTMatch[] Matches,
        ASTExpression Default)
        : ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children { 
            get
            {
                List<ASTNode> cld = [Object];
                foreach (ASTMatch match in Matches)
                {
                    cld.Add(match.In);
                    cld.Add(match.Out);
                }
                cld.Add(Default);
                return cld;
            }
        }
    }
}
