using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Types
{
    public record ASTPosition(Position Start, Position End)
    {
        public override string ToString()
        {
            return $"{Start.Filename} {Start.Line}:{Start.Column} {End.Line}:{End.Column}";
        }
    }
}
