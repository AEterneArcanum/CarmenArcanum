using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Loaded
{

    public record CodeFile(
        string Name,
        string[] Imports,
        AST.ASTNode[] Nodes);

    /// <summary>
    /// Manage loading files into ast and grabbing / filling imports from the ast -- output as single collection of all ast nodes
    /// </summary>
    public class Loader
    {
        public Logging.Logger LogBook { get; init; } = new();

        public Dictionary<string, CodeFile> LoadedFiles { get; init; } = [];

        public void Log(string message)
        {
            LogBook.Log(message, Logging.LogLevel.Info);
        }

        public void Debug(string message)
        {
            LogBook.Log(message);
        }

        public Loader()
        {

        }
        /// <summary>
        /// Load a file if it isnt loaded, get imports from the file, load the imports
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="project">root directory of working project</param>
        /// <param name="include">directories to search for included files.</param>
        public void LoadFile(string filename, string project, string[] include)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                Log("No filename passed.");
                return;
            }
            if (string.IsNullOrWhiteSpace(project))
            {
                Log("No project directory set."); 
                return;
            }
            if (LoadedFiles.ContainsKey(filename))
            {
                LogBook.Log($"File {filename} already being loaded.");
                return;
            }
            // Find File
            string pth = string.Empty;
            if (!File.Exists($"{filename}/{project}"))
            {
                foreach(string s in include)
                {
                    if (File.Exists($"{filename}/{s}"))
                    {
                        pth = $"{filename}/{s}";
                        break;
                    }
                }
            }
            else
            {
                pth = $"{filename}/{project}";
            }
            if (pth == string.Empty)
            {
                Log($"File {filename} does not exist.");
                return;
            }
            // Load file.
            string content = File.ReadAllText(pth);
            // Parse tokens.
            var rawTokens = Lexer.Lexer.Parse(content, $"{project}/{filename}");
            // Parse to AST.
            var parser = new Parser.CarmenParser();
            parser.ParseTokens([..rawTokens]);
            // On Parser success get usings
            var imports = parser.ParsedNodes.OfType<AST.Statements.ASTImport>().Select(x => x.Import.Value);
            LoadedFiles.Add(
                filename, 
                new(filename,
                    [..imports],
                    [..parser.ParsedNodes]));
            // With the file added recurse through the called imports
            foreach(var import in imports)
            {
                LoadFile(import, project, include);
            }
        }
    }
}
