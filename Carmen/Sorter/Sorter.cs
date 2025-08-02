using Arcane.Carmen.AST;
using Arcane.Carmen.AST.Statements;
using Arcane.Carmen.Loaded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Sorter
{


    /// <summary>
    /// Sort a collection of ast nodes for the validator
    /// </summary>
    public class Sorter
    {
        private struct SorterBucket
        {
            public List<ASTStructure> Structures16;
            public List<ASTStructure> Structures32;
            public List<ASTStructure> Structures64;

            public List<ASTFunction> Functions16;
            public List<ASTFunction> Functions32;
            public List<ASTFunction> Functions64;

            public List<ASTVariableDefinition> Variables16;
            public List<ASTVariableDefinition> Variables32;
            public List<ASTVariableDefinition> Variables64;

            public ASTEntryPoint EntryPoint;
        }

        private SorterBucket bucket;

        public ASTEnvironment Sort(string baseFile, Dictionary<string, CodeFile> loadedFiles)
        {
            bucket = new SorterBucket();

            foreach (var file in loadedFiles.Values)
                SortFile(file, file.Name == baseFile);

            // Arch16
            var a16 = new ASTArchZone(AST.Types.ASTArchitecture.Arch16,
                [.. bucket.Structures16 ?? []],
                [.. bucket.Functions16 ?? []],
                [.. bucket.Variables16 ?? []]);
            // Arch32
            var a32 = new ASTArchZone(AST.Types.ASTArchitecture.Arch32,
                [.. bucket.Structures32 ?? []],
                [.. bucket.Functions32 ?? []],
                [.. bucket.Variables32 ?? []]);
            // Arch64
            var a64 = new ASTArchZone(AST.Types.ASTArchitecture.Arch64,
                [.. bucket.Structures64 ?? []],
                [.. bucket.Functions64 ?? []],
                [.. bucket.Variables64 ?? []]);

            if (bucket.EntryPoint == null)
            {
                // Warn
                throw new Exception();
            }

            var env = new ASTEnvironment([a16, a32, a64], bucket.EntryPoint);
            return env;
        }

        private void SortFile(CodeFile file, bool ovewriteEP)
        {

            foreach (var node in file.Nodes)
            {
                switch (node)
                {
                    case ASTStructure structure:
                        bucket.Structures64.Add(structure);
                        break;
                    case ASTVariableDefinition variableDefinition:
                        bucket.Variables64.Add(variableDefinition);
                        break;
                    case ASTFunction function:
                        bucket.Functions64.Add(function);
                        break;
                    case ASTEntryPoint entrypoint:
                        if (ovewriteEP) bucket.EntryPoint = entrypoint;
                        break;
                    case ASTArchDefined aSTArchDefined:
                        SortArchDefined(aSTArchDefined);
                        break;
                    default:
                        break; // Do we reach here?
                }
            }

        }

        private void SortArchDefined(ASTArchDefined archDefined)
        {
            switch (archDefined.Statement)
            {
                case ASTStructure structure:
                    switch (archDefined.Architecture)
                    {
                        case AST.Types.ASTArchitecture.Arch16:
                            bucket.Structures16.Add(structure); break;
                        case AST.Types.ASTArchitecture.Arch32:
                            bucket.Structures32.Add(structure); break;
                        case AST.Types.ASTArchitecture.Arch64:
                            bucket.Structures64.Add(structure); break;
                        default:
                            break; // Error?
                    }
                    break;
                case ASTVariableDefinition variableDefinition:
                    switch (archDefined.Architecture)
                    {
                        case AST.Types.ASTArchitecture.Arch16:
                            bucket.Variables16.Add(variableDefinition); break;
                        case AST.Types.ASTArchitecture.Arch32:
                            bucket.Variables32.Add(variableDefinition); break;
                        case AST.Types.ASTArchitecture.Arch64:
                            bucket.Variables64.Add(variableDefinition); break;
                        default:
                            break; // Error?
                    }
                    break;
                case ASTFunction function:
                    switch (archDefined.Architecture)
                    {
                        case AST.Types.ASTArchitecture.Arch16:
                            bucket.Functions16.Add(function); break;
                        case AST.Types.ASTArchitecture.Arch32:
                            bucket.Functions32.Add(function); break;
                        case AST.Types.ASTArchitecture.Arch64:
                            bucket.Functions64.Add(function); break;
                        default:
                            break; // Error?
                    }
                    break;
                case ASTBlock block:
                    SortASTBlock(block, archDefined.Architecture);
                    break;
                default:
                    break;
            }
        }

        private void SortASTBlock(ASTBlock block, AST.Types.ASTArchitecture architecture)
        {
            foreach (var node in block.InnerNodes)
            {
                switch (node)
                {
                    case ASTStructure structure:
                        switch (architecture)
                        {
                            case AST.Types.ASTArchitecture.Arch16:
                                bucket.Structures16.Add(structure); break;
                            case AST.Types.ASTArchitecture.Arch32:
                                bucket.Structures32.Add(structure); break;
                            case AST.Types.ASTArchitecture.Arch64:
                                bucket.Structures64.Add(structure); break;
                            default: break;
                        }
                        break;
                    case ASTFunction func:
                        switch (architecture)
                        {
                            case AST.Types.ASTArchitecture.Arch16:
                                bucket.Functions16.Add(func); break;
                            case AST.Types.ASTArchitecture.Arch32:
                                bucket.Functions32.Add(func); break;
                            case AST.Types.ASTArchitecture.Arch64:
                                bucket.Functions64.Add(func); break;
                            default: break;
                        }
                        break;
                    case ASTVariableDefinition variableDefinition:
                        switch (architecture)
                        {
                            case AST.Types.ASTArchitecture.Arch16:
                                bucket.Variables16.Add(variableDefinition); break;
                            case AST.Types.ASTArchitecture.Arch32:
                                bucket.Variables32.Add(variableDefinition); break;
                            case AST.Types.ASTArchitecture.Arch64:
                                bucket.Variables64.Add(variableDefinition); break;
                            default: break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
