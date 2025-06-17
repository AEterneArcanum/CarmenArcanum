public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Filename Required For Parsing.");
            return;
        }
        if (!File.Exists(args[0]))
        {
            Console.WriteLine($"File {args[0]} does not exist.");
            return;
        }
        string testScript = File.ReadAllText(args[0]);
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
