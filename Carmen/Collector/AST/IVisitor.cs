using Arcane.Carmen.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST
{
    public interface IVisitor
    {
        public void Visit(IVisitable visitable);
    }
}
