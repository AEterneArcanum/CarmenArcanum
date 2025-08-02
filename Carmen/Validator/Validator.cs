using Arcane.Carmen.AST;
using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.AST.Statements;

namespace Arcane.Carmen.Validator
{
    public class FunctionReference
    {
        public string Name { get => Function.Identifier.Identifier; }
        public int ReferenceCount { get; private set; } = 0;
        public required ASTFunction Function { get; init; }
        public void AddReference() { ReferenceCount++; }

        public required ParameterReference[] Parameters { get; init; }
    }

    public class ParameterReference
    {
        public string Name { get => Parameter.Identifier.Identifier; }
        public int ReferenceCount { get; private set; } = 0;
        public required ASTFuncParamDecl Parameter { get; init; }
        public void AddReference() { ReferenceCount++; }
    }

    public class StructureReference
    {
        public string Name { get => Structure.Identifier.Identifier; }
        public int ReferenceCount { get; private set; } = 0;
        public required ASTStructure Structure { get; init; }
        public void AddReference() { ReferenceCount++; }

        public required VariableReference[] MemberVariables { get; init; }
    }

    public class VariableReference
    {
        public string Name { get => Structure.Identifier.Identifier; }
        public int ReferenceCount { get; private set; } = 0;
        public required ASTVariableDefinition Structure { get; init; }
        public void AddReference() { ReferenceCount++; }
    }

    public class BlockEnvironment 
    {
        public required object Owner { get; init; } // Owning ast node or env for ep.
        public required Environment Environment { get; init; } // Full scope app environment
        public BlockEnvironment? Parent { get; init; } // Parent block null for function blocks and ep.

        public required ASTBlock Block { get; init; } // Raw ast
        /// <summary>
        /// Locally defined variables.
        /// </summary>
        public required VariableReference[] VariableReference { get; init; }
    }

    public class Environment
    {
        public readonly Dictionary<string, StructureReference> Structures = [];

        public readonly Dictionary<string, FunctionReference> Functions = [];

        public readonly Dictionary<string, VariableReference> Variables = [];

        public readonly Dictionary<object, BlockEnvironment> CodeBlocks = [];

        public readonly BlockEnvironment EntryPointBlock;

        public Environment(ASTEnvironment env)
        {
            // owner + env = this
            // parent null
            // block
            ASTBlock? aSTBlock = null;
            if (env.EntryPoint.Code is ASTBlock blk)
            {
                aSTBlock = blk;
            }
            else
            {
                aSTBlock = new ASTBlock(env.EntryPoint.Code.Position, [env.EntryPoint.Code]);
            }
            // Look for locally defined variables
            List<ASTVariableDefinition> vd = [];
            foreach(var node in aSTBlock.InnerNodes)
            {

            }


        }
    }
}
