using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record ASTFor(
        ASTPosition Position,
        ASTIdentifier Indexer,
        ASTExpression Start,
        ASTExpression Condition,
        bool IsUntil, // Expression used until, flipping the condition while true vs until true
        ASTExpression? Step,
        ASTBlock Body)
        : ASTStatement(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children { 
            get
            {
                List<ASTNode> inner = [
                    Indexer,
                    Start, 
                    Condition, 
                    Step, 
                    Body
                    ];
                return inner;
            }
        }
    }
}
