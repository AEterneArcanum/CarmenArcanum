
using Arcane.Carmen.Lexer;
using Arcane.Carmen.Parser;
using Arcane.Carmen.Writer;

namespace Testing
{
    internal class Program
    {
        const string DEFAULT_IN_FILE = "testScript.txt";
        const string DEFAULT_OUT_FILE = "out";

        internal enum Writers { CS }

        static void Main(string[] args)
        {
            string file_in = DEFAULT_IN_FILE;
            if (args.Length >= 1)
            {
                if (!File.Exists(args[0]))
                {
                    Console.WriteLine($"Error in input filename {args[0]}.");
                    return;
                }
                file_in = args[0];
            }
            string file_out = DEFAULT_OUT_FILE;
            if (args.Length >= 2)
            {
                file_out = args[1];
            }

            bool noisy = false;
            Writers wrtr = Writers.CS;
            if (args.Length >= 3)
            {
                for (int i = 2; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-n":
                            noisy = true; break;
                        case "-cs":
                            wrtr = Writers.CS; break;
                        default:
                            Console.WriteLine($"Unrecognized argument {args[i]} passed.");
                            return;
                    }
                }
            }


            // Load tokens
            Console.WriteLine($"Loading file {file_in}...");
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
                Console.WriteLine($"{t.Function} :: {t.Message}");
                Console.WriteLine(t.Tokens.AsString());
            };
            parser.LogBook.OnEntry += (t) =>
            {
                if (noisy)
                {
                    Console.WriteLine($"{t.Created} - {t.Message}");
                }
                else
                {
                    if (t.Level != Arcane.Carmen.Logging.LogLevel.Debug)
                    {
                        Console.WriteLine($"{t.Created} - {t.Message}");
                    }
                }
            };
            parser.ParseTokens([..toks]);

            Writer writer = wrtr switch
            {
                Writers.CS => new CSWriter(),
                _ => new CSWriter() // Default to cs for now.
            };

            Console.WriteLine($"Writing to file {file_out}...");
            bool writerErrored = false;
            writer.OnWriterError += (t) => { Console.WriteLine(t); writerErrored = true; };
            writer.ValidateNodes([.. parser.ParsedNodes]);
            if (writerErrored) { Console.WriteLine("Invalid nodes in ast."); return; }
            try
            {
                writer.WriteNodes([..parser.ParsedNodes]);
                writer.SaveFile(file_out);
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
