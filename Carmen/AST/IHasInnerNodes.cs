using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST
{
    /// <summary>
    /// Contains other nodes.
    /// </summary>
    interface IHasInnerNodes
    {
        /// <summary>
        /// Contained nodes.
        /// </summary>
        public IEnumerable<ASTNode> Children { get; }
    }
}
