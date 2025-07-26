
using Arcane.Carmen.Lexer;
using Arcane.Carmen.Parser;
using Arcane.Carmen.Writer;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fn = "testScript.txt";
            string fo = "out.cs";
            bool noisy = false;
            // Load tokens
            Console.WriteLine($"Loading file {fn}...");
            string content = File.ReadAllText("testScript.txt");

            Console.WriteLine($"Text read:");
            Console.WriteLine(content);

            Console.WriteLine($"Parsing content...");
            var tokens = Lexer.Parse(content, "testScript.txt");

            Console.WriteLine("Tokens parsed:");
            foreach (var token in tokens) 
            {
                if (!string.IsNullOrWhiteSpace(token.Content))
                {
                    Console.WriteLine(token.Content);
                }
            }

            Console.WriteLine($"Trimming comments and whitespace...");
            var toks = tokens.Where(t =>
            t.Type != TokenType.Comment &&
            t.Type != TokenType.Whitespace).ToList();

            Console.WriteLine($"Remaining tokens: {toks.Count}");
            // Parse to ast
            Console.WriteLine($"Parsing tokens...");
            var parser = new CarmenParser();
            parser.OnParserError += (t) =>
            {
                Console.WriteLine($"{t.Function} :: {t.message}");
                Console.WriteLine(t.Tokens.AsString());
            };
            parser.OnParserLog += (t) =>
            {
                if (noisy)
                {
                    Console.WriteLine($"{t.Time} - {t.Log}");
                }
                else
                {
                    if (t.Level != LogLevel.Noise)
                    {
                        Console.WriteLine($"{t.Time} - {t.Log}");
                    }
                }
            };
            parser.ParseTokens([..toks]);



            Console.WriteLine($"Writing to file {fo}...");
            var writer = new CSWriter();
            try
            {
                writer.Write([..parser.ParsedNodes], fo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            Console.WriteLine($"Transcription completed successfully.");
        }

        //static void LookAtParsed() // <-- first successful transcribe
        //{
        //    byte vx;
        //    vx = 5;
        //lstart:;
        //    if ((vx == 0)) goto lend;
        //    --vx;
        //    goto lstart;
        //lend:;
        //}
    }
}
