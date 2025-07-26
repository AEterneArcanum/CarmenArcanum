using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST;

public record ASTDecrement(Position Position, ASTExpression Expression, bool IsPrefix) : ASTExpression(Position), IStandalone;
