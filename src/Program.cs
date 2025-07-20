using Arcane.Script.Carmen;

public static class Program
{
    public static void Main(string[] args)
    {
        string fn = string.Empty;
        if (args.Length == 0)
        {
            Console.WriteLine("No file specified. Please provide a script file to execute.");
            return;
        }
        if (args.Length >= 1) { 
            fn = args[0];
            if (!File.Exists(fn))
            {
                Console.WriteLine($"File {fn} does not exist.");
                return;
            }
        }
        string testScript = File.ReadAllText(fn);
        //Console.WriteLine($"TestScript: {testScript}");
        var tokens = Lexer.Tokenize(testScript);
        // foreach (var token in tokens)
        // {
        // Console.WriteLine($"{token.Type} :: {token.Value} :: {token.Line} :: {token.Column}");
        // }
        var parsed = Parser.Parse([.. tokens]);
        //foreach (var p in parsed)
        //{
        //    Console.WriteLine(p.ToString());
        //}
        Interpreter interpreter = new(parsed);
        interpreter.Execute();
    }
}
