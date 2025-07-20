using Arcane.Carmen.Lexer.Tokens;
using System.Reflection;

namespace Arcane.Carmen.AST
{
    public class ExpressionHandler
    {
        public List<ExpressionParser> Parsers { get; }

        public ExpressionHandler()
        {
            Parsers = new List<ExpressionParser>();
            // Initialize with all parsers
            Parsers = [.. Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ExpressionParser)))
                .Where(t => !t.IsAbstract)
                .Select(t => (ExpressionParser)Activator.CreateInstance(t)!)];
            Parsers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens == null || tokens.Length == 0)
            {
                return false; // No tokens to parse
            }
            foreach (var parser in Parsers)
            {
                if (parser.TryParse(tokens, out var expr))
                {
                    expression = expr;
                    return true;
                }
            }
            return false; // No parser could handle the tokens
        }
    }
}
