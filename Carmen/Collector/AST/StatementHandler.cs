using Arcane.Carmen.Collector.Lexer.Tokens;
using System.Reflection;

namespace Arcane.Carmen.Collector.AST
{
    public class StatementHandler
    {
        public List<StatementParser> Parsers { get; }

        public StatementHandler()
        {
            Parsers = new List<StatementParser>();
            Parsers = [..Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(StatementParser)))
                .Where(t => !t.IsAbstract)
                .Select(t => (StatementParser)Activator.CreateInstance(t)!)];
            Parsers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public bool TryParse(Token[] tokens, out Statement? statement)
        {
            statement = null;
            if (tokens == null || tokens.Length == 0)
            {
                return false; // No tokens to parse
            }
            foreach (var parser in Parsers)
            {
                if (parser.TryParse(tokens, out var stmt))
                {
                    statement = stmt;
                    return true;
                }
            }
            return false; // No parser could handle the tokens
        }
    }
}
