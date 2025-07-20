using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.Lexer;

namespace Testing
{
    internal class Program
    {
        static bool failedOnly = true;

        public static string[] TestNumbers =
        [
            "0", "1", "123", "1234567890", "-1", "-123", "-1234567890",
            "0.0", "1.0", "123.456", "-1.0", "-123.456", "first", "second",
            "twenty first", "one", "one hundred", "one hundredth", "seventy second", "twenty five",
            "two hundred fifty six point five"
        ];
        public static string[] TestStrings =
        [
            "\"\"", "\"a\"", "\"abc\"", "\"Hello, World!\"",
            "\"This is a string with a newline\\ncharacter.\"",
            "\"This is a string with a tab\\tcharacter.\"",
            "\"This is a string with a backslash\\\\character.\"",
            "\"This is a string with a quote \\\" character.\"",
            "\"This is a string\" \"This is another\""
        ];
        public static string[] TestCharacters =
        [
            "'a'", "'b'", "'c'", "'\\n'", "'\\t'", "'\\r'", "'\\''", "'\\\\'", "'\"'"
        ];
        public static string[] TestIdentifiers =
        [
            "$a", "$abc", "$HelloWorld", "$thisIsAVariable", "$myVariable123", "$another_variable",
            "$variable_with_underscores", "$variableWithNumbers123", "$variableWithSpecialChars"
        ];
        public static string[] TestLists = [
            "; 25, 5, 43, 23, and 12", "; 1", "; 3, and four"
            //"[]", "[1]", "[1, 2, 3]", "[\"a\", \"b\", \"c\"]", "[1, \"a\", 2, \"b\"]",
            //"[\"Hello\", \"World\"]", "[1, 2, 3, 4, 5]", "[\"one\", \"two\", \"three\"]"
        ];
        public static string[] TestIndices =
        [
            "index 0", "index 1", "index 2", "index 3", "index 4",
            "index 5", "index 6", "index 7", "index 8", "index 9",
            //"index -1", "index -2", "index -3",
            "index seven", "sixth index from last", "index 9 from last",
            "index $idxOfOf"
        ];
        public static string[] TestArrayAccesses =
        [
            "the index 0 of $myArray", "the index 1 of $myArray", "the index 2 of $myArray",
            "the index 3 of $myArray", "the index 4 of $myArray", "the index 5 of $myArray",
            "the index 6 of $myArray", "the index 7 of $myArray", "the index 8 of $myArray",
            "the index 9 of $myArray", "the index $x from last of $myArray",
            "the index zero of the index one from last of $myArray", // Example of nested index access
            "the (the index seven of $arrayOne) index from last of $arrayTwo", // Example of nested array access
            //"index -1 of $myArray", "index -2 of $myArray",
            //"index -3 of $myArray"
        ];

        private static void _testArrayAccessParser()
        {
            foreach (var access in TestArrayAccesses)
            {
                if (!failedOnly) Console.Write($"Testing: {access}     ");
                var tokens = Tokenizer.Tokenize(access);
                if (ExprArrayAccess.TryParse([.. tokens], out var exprArrayAccess))
                {
                    if (!failedOnly) Console.WriteLine($"Parsed Expression: {exprArrayAccess?.ToString()}");
                }
                else
                {
                    Console.WriteLine("Failed to parse expression.");
                }
            }
        }

        private static void _testIndexParser()
        {
            foreach (var index in TestIndices)
            {
                if (!failedOnly) Console.Write($"Testing: {index}     ");
                var tokens = Tokenizer.Tokenize(index);
                if (ExprIndex.TryParse([.. tokens], out var exprIndex))
                {
                    if (!failedOnly) Console.WriteLine($"Parsed Expression: {exprIndex?.ToString()}");
                }
                else
                {
                    Console.WriteLine("Failed to parse expression.");
                }
            }
        }

        private static void _testListParser()
        {
            foreach (var list in TestLists)
            {
                if (!failedOnly) Console.WriteLine($"Testing: {list}     ");
                var tokens = Tokenizer.Tokenize(list);
                if (ExprList.TryParse([.. tokens], out var exprList))
                {
                    if (!failedOnly) Console.WriteLine($"Parsed Expression: {exprList?.ToString()}");
                }
                else
                {
                    Console.WriteLine("Failed to parse expression.");
                }
            }
        }

        private static void _testIdentifierParser()
        {
            foreach (var identifier in TestIdentifiers)
            {
                if (!failedOnly) Console.Write($"Testing: {identifier}     ");
                var tokens = Tokenizer.Tokenize(identifier);
                if (ExprIdentifier.TryParse([.. tokens], out var exprIdentifier))
                {
                    if (!failedOnly) Console.WriteLine($"Parsed Expression: {exprIdentifier}");
                }
                else
                {
                    Console.WriteLine("Failed to parse expression.");
                }
            }
        }

        private static void _testCharParser()
        {
            foreach (var ch in TestCharacters)
            {
                if (!failedOnly) Console.Write($"Testing: {ch}     ");
                var tokens = Tokenizer.Tokenize(ch);
                foreach (var token in tokens)
                {
                    if (ExprCharLiteral.TryParse([token], out var value))
                    {
                        if (!failedOnly) Console.WriteLine($"Parsed Expression: {value}");
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse expression.");
                    }
                }
            }
        }

        private static void _testStringParser()
        {
            foreach (var str in TestStrings)
            {
                if (!failedOnly) Console.Write($"Testing: {str}     ");
                var tokens = Tokenizer.Tokenize(str);
                foreach (var token in tokens)
                {
                    if (ExprStringLiteral.TryParse([token] , out var value))
                    {
                        if (!failedOnly) Console.WriteLine($"Parsed Expression: {value}");
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse expression.");
                    }
                }
            }
        }

        private static void _testNumberParser()
        {
            foreach (var number in TestNumbers)
            {
                if (!failedOnly) Console.Write($"Testing: {number}    ");
                var tokens = Tokenizer.Tokenize(number);
                if (ExprNumberLiteral.TryParse([..tokens], out var exprNumber))
                {
                    if (!failedOnly) Console.WriteLine($"Parsed Expression: {exprNumber}");
                }
                else
                {
                    Console.WriteLine("Failed to parse expression.");
                }
            }
        }

        static void Main(string[] args)
        {
            _testArrayAccessParser();
            _testNumberParser();
            _testStringParser();
            _testCharParser();
            _testIdentifierParser();
            _testListParser();
            _testIndexParser();
        }
    }
}
