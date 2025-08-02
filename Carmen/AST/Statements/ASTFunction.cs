using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record ASTFunction(
        ASTPosition Position,
        ASTIdentifier Identifier,
        bool Inline,
        ASTArchitecture? Architecture,
        ASTIdentifier? LinkedStruct, // Like CS Extentions
        ASTTypeInfo? ReturnType,
        ASTFuncParamDecl[] Parameters,
        ASTBlock Body)
        : ASTStatement(Position), IHasInnerNodes
    {
        public IEnumerable<ASTNode> Children 
            => (LinkedStruct is null) ? [Identifier, ..Parameters, Body]
            : [Identifier, LinkedStruct, ..Parameters, Body];
    }
}
