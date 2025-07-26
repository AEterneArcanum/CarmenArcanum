using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Arcane.Carmen.Collector.AST
{
    public class ASTSymbolTable<T> where T : class
    {
        private readonly ConcurrentDictionary<string, T> _symbols = [];
        private readonly ConcurrentDictionary<ASTBlock, ConcurrentDictionary<string, T>> _scopedSymbols = [];

        public bool TryGetSymbol(string name, ASTBlock? block, [NotNullWhen(true)] out T? symbol)
        {
            symbol = default;

            if (_symbols.TryGetValue(name, out symbol)) 
            {
                return true;
            }

            if (block is null) return false;
            
            bool found = false;
            while (!found)
            {
                if (!_scopedSymbols.ContainsKey(block)) break;
                if (_scopedSymbols[block].TryGetValue(name, out symbol))
                {
                    return true;
                }
                else
                {
                    if (block.ParentBlock is not null)
                        block = block.ParentBlock;
                    else
                        break;
                }
            }

            return false;
        }
    }

    public class TypeInfo
    {
        public required string Name { get; init; }
        public bool IsPrimitive { get; init; }
        public TypeInfo? BaseType { get; init; }
        public Dictionary<string, GenericTypeParameter> GenericParameters { get; } = [];
    }

    public class GenericTypeParameter
    {
        public required string Name { get; init; }
        public TypeInfo[]? Restrictions { get; init; } // Parameter must inherit these
    }


    /// <summary>
    /// Stores a parsed and sorted AST at the top level
    /// </summary>
    public class ASTEnvironment
    {
        public ASTEnvironment() { EntryPoint = new(this, -1, ASTBlockType.Function); }
        public ASTSymbolTable<ASTStructRef> Structures { get; init; } = new();
        public ASTSymbolTable<ASTFunctionRef> Functions { get; init; } = new();
        public ASTSymbolTable<ASTVarRef> Variables { get; init; } = new();
        public ASTSymbolTable<ASTLabelRef> Labels { get; init; } = new();
        public Dictionary<string, TypeInfo> Types { get; init; } = [];
        public ASTBlock EntryPoint { get; private set; }
        public List<ASTParseException> Errors { get; } = [];

        public void SetEntryPoint(ASTBlock newEntry) // Parser needs to add this to environment
        {
            EntryPoint = newEntry;
        }
    }
    public class ASTStructRef
    {
        public required ExStructDef Definition { get; init; }
        public Dictionary<string, ASTVarRef> VarRefs { get; init; } = [];
        public Dictionary<string, ASTFunctionRef> Functions { get; init; } = [];
    }
    public class ASTFunctionRef
    {
        public required ExFuncDef Definition { get; init; }
        public ASTBlock? BodyBlock { get; set; }
        public Dictionary<string, ASTVarRef> Parameters { get; init; } = [];
        public Dictionary<string, ASTVarRef> LocalVariables { get; init; } = [];
    }
    public enum ASTBlockType
    {
        Function, // Main
        IfElse,
        SwitchCase,
        Loop
    }

    /// <summary>
    /// Structure for testing access and definition access in code
    /// </summary>
    public class ASTBlock
    {
        public ASTEnvironment Environment { get; init; }
        public int Index { get; init; } // index of owning code within parent block // the if / while statement
        public ASTBlockType Type { get; init; }
        public ASTBlock? ParentBlock { get; init; }
        public List<ASTNode> Code { get; init; } = [];
        public List<ASTBlock> SubBlocks { get; init; } = []; // iner environments of while for loop and
        public ASTBlock(ASTEnvironment environment, int index, ASTBlockType type, ASTBlock? parent = null)
        {
            Environment = environment;
            Type = type;
            ParentBlock = parent;
        }
        public bool TryResolveVar(string name, [NotNullWhen(true)] out ASTVarRef? varRef)
        {
            if (Environment.Variables.TryGetSymbol(name, this, out varRef))
                return true;

            return ParentBlock?.TryResolveVar(name, out varRef) ?? false;
        }
    }
    public class ASTLabelRef
    {
        public required ExLabel LabelDefinition { get; init; }
        public int IndexOfLabel { get; init; } // index of label statement
    }
    public class ASTVarRef
    {
        public required ExVarDef VariableDefinition { get; init; }
        /// <summary>
        /// Line / Index of definition necessary for checking if defined in position. 
        /// Parameters in functions should be given a negative value here.
        /// </summary>
        public int IndexOfDefinition { get; init; } // index of definition statement
        public VarType Type { get; init; } // ordinary/constant/static/parameter
        public Dictionary<ASTBlock, int> UsageLines { get; init; } = [];
    }
}
