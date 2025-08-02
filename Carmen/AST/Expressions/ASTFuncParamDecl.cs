using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public enum ASTReadWriteMod
    {
        ReadWrite,
        ReadOnly,
        WriteOnly
    }

    public record ASTFuncParamDecl(
        ASTPosition Position,
        ASTIdentifier Identifier,
        ASTTypeInfo TypeInfo,
        ASTExpression? Default,
        ASTReadWriteMod RWModifier,
        bool ByReference,
        bool IsRestrict) :
        ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children 
            => (Default is null)?[Identifier]:[Identifier, Default];
    }

    // 'restrict'? 'ref'? 'in'|'out'? EXPRESSION
    public record ASTFuncParamImpl(
        ASTPosition Position,
        ASTIdentifier Identifier,
        ASTReadWriteMod RWModifier,
        bool ByReference,
        bool IsRestrict)
        : ASTExpression(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children 
            => [Identifier];
    }
}
