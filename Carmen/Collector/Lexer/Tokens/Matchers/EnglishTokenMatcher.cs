using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.Lexer.Tokens.Matchers
{
    public class EnglishTokenMatcher
        : ITokenMatcher
    {
        // String representations of numbers
        #region DIGITS
        // String representations of locational numbers
        public const string First = "first";
        public const string Second = "second";
        public const string Third = "third";
        public const string Fourth = "fourth";
        public const string Fifth = "fifth";
        public const string Sixth = "sixth";
        public const string Seventh = "seventh";
        public const string Eighth = "eighth";
        public const string Ninth = "ninth";
        public const string Tenth = "tenth";
        public const string Eleventh = "eleventh";
        public const string Twelfth = "twelfth";
        public const string Thirteenth = "thirteenth";
        public const string Fourteenth = "fourteenth";
        public const string Fifteenth = "fifteenth";
        public const string Sixteenth = "sixteenth";
        public const string Seventeenth = "seventeenth";
        public const string Eighteenth = "eighteenth";
        public const string Nineteenth = "nineteenth";
        public const string Twentieth = "twentieth";
        public const string Thirtieth = "thirtieth"; // 30th
        public const string Fortieth = "fortieth"; // 40th
        public const string Fiftieth = "fiftieth"; // 50th
        public const string Sixtieth = "sixtieth"; // 60th
        public const string Seventieth = "seventieth"; // 70th
        public const string Eightieth = "eightieth"; // 80th
        public const string Ninetieth = "ninetieth"; // 90th
        public const string Hundredth = "hundredth"; // 100th
        public const string Thousandth = "thousandth"; // 1000th
        public const string Millionth = "millionth"; // 1,000,000th
        public const string Billionth = "billionth"; // 1,000,000,000th
        public const string Trillionth = "trillionth"; // 1,000,000,000,000th
        public const string Quadrillionth = "quadrillionth"; // 1,000,000,000,000,000th
        public const string Quintillionth = "quintillionth"; // 1,000,000,000,000,000,000th
        // These are the string representations of numbers from 0 to 20 and multiples of ten up to ninety.
        public const string One = "one";
        public const string Two = "two";
        public const string Three = "three";
        public const string Four = "four";
        public const string Five = "five";
        public const string Six = "six";
        public const string Seven = "seven";
        public const string Eight = "eight";
        public const string Nine = "nine";
        public const string Zero = "zero";
        public const string Ten = "ten";
        public const string Eleven = "eleven";
        public const string Twelve = "twelve";
        public const string Thirteen = "thirteen";
        public const string Fourteen = "fourteen";
        public const string Fifteen = "fifteen";
        public const string Sixteen = "sixteen";
        public const string Seventeen = "seventeen";
        public const string Eighteen = "eighteen";
        public const string Nineteen = "nineteen";
        public const string Twenty = "twenty";
        public const string Thirty = "thirty";
        public const string Forty = "forty";
        public const string Fifty = "fifty";
        public const string Sixty = "sixty";
        public const string Seventy = "seventy";
        public const string Eighty = "eighty";
        public const string Ninety = "ninety";
        public const string Hundred = "hundred";
        public const string Thousand = "thousand";
        public const string Million = "million";
        public const string Billion = "billion";
        public const string Trillion = "trillion";
        public const string Quadrillion = "quadrillion";
        public const string Quintillion = "quintillion";
        // ROMOVED DUE TO OVERSIZE
        //public const string Sextillion = "sextillion";
        //public const string Septillion = "septillion";
        //public const string Octillion = "octillion";
        //public const string Nonillion = "nonillion";
        //public const string Decillion = "decillion";
        //public const string Undecillion = "undecillion";
        //public const string Duodecillion = "duodecillion";
        //public const string Tredecillion = "tredecillion";
        //public const string Quattuordecillion = "quattuordecillion";
        //public const string Quindecillion = "quindecillion";
        //public const string Sexdecillion = "sexdecillion";
        //public const string Septendecillion = "septendecillion";
        //public const string Octodecillion = "octodecillion";
        //public const string Novemdecillion = "novemdecillion";
        //public const string Vigintillion = "vigintillion";
        //public const string Unvigintillion = "unvigintillion";
        //public const string Duovigintillion = "duovigintillion";
        //public const string Trevigintillion = "trevigintillion";
        public const string Point = "point"; // Decimal point in numbers
        public const string Negative = "negative"; // Negative sign for numbers
        #endregion
        #region KEYWORDS
        public const string VariableIdentifier = "$"; // Used for variables, e.g., "$myVariable"
        public const string FunctionIdentifier = "@"; // Used for functions, e.g., "@myFunction"
        public const string StructurIdentifier = "#"; // Used for structures, e.g., "#MyStruct", "#MyEnum"
        public const string LabelIdentifier = ":"; // Used for labels in code, e.g., ":myLabel"
        public const string AliasIdentifier = "_"; // Used for aliases, e.g., "_myAlias"

        public const string True = "true";
        public const string False = "false";
        public const string Null = "null";

        public const string TypeString = "string";
        public const string TypeBoolean = "boolean";
        public const string TypeByte = "byte";
        public const string TypeChar = "char";
        public const string TypeShort = "short";
        public const string TypeInt = "integer";
        public const string TypeLong = "long";
        public const string TypeFloat = "single";
        public const string TypeDouble = "double";
        public const string TypeDecimal = "decimal";
        public const string TypeObject = "object";
        public const string TypeEnum = "enum";

        public const string TypeStructure = "structure";

        public const string Program = "program";
        public const string Define = "define";
        public const string Is = "is";
        public const string Index = "index"; // Used for array access and is value keyword in Carmer iterators
        public const string A = "a";
        public const string An = "an";
        public const string And = "and";
        public const string As = "as";
        public const string By = "by"; // Used for iterators, e.g., "by index" or "by value"
        public const string Equal = "equal";
        public const string Of = "of";
        public const string To = "to";
        public const string From = "from";
        public const string Last = "last"; // Used for iterators
        public const string The = "the"; // Used for iterators, e.g., "the last item"
        public const string Item = "item"; // Used for indexers, e.g., "the last item" & used in iterators item.Value = "item value"; within iterators
        public const string Final = "final"; // Used for iterators, e.g., "the final item"
        public const string Initial = "initial"; // Used for iterators, e.g., "the initial item"
        public const string Not = "not"; // Used for boolean negation, e.g., "not true" or "not false"
        public const string Or = "or"; // Used for boolean operations, e.g., "true or false"
        public const string Xor = "xor"; // Used for boolean exclusive or, e.g., "true xor false"
        public const string LogicalXOR = "⊕"; // Used for boolean exclusive or, e.g., "true ⊕ false"
        public const string Exclusive = "exclusive"; // Used for boolean exclusive or, e.g., "true exclusive ..or.. false"
        public const string Greater = "greater"; // Used for comparison, e.g., "n greater than m"
        public const string Less = "less"; // Used for comparison, e.g., "n less than m"
        public const string Than = "than"; // Used for comparison, e.g., "n greater than m" or "n less than m"
        public const string If = "if"; // Used for conditional expressions, e.g., "if n is greater than m then ..."
        public const string Then = "then"; // Used for conditional expressions, e.g., "if n is greater than m then ..."
        public const string Call = "call"; // Used for function calls, e.g., "call myFunction with parameters"
        public const string With = "with"; // Used for function calls, e.g., "call myFunction with parameters"
        public const string Elements = "elements"; // Used for iterators, e.g., "the elements of the array" or "the elements of the list"
        public const string At = "at"; // Used for accessing elements at a specific index, e.g., "the element at index 0 of the array"
        public const string Through = "through"; // Used for iterators, e.g., "the elements of the array through index 5"
        public const string Beginning = "beginning"; // Used for iterators, e.g., "the beginning of the array" or "the first element of the array"
        public const string Ending = "ending";
        public const string Value = "value";
        public const string Each = "each"; // Used for iterators, e.g., "each item in the array" or "each element in the list"
        public const string Case = "case";

        public const string Step = "step"; // Used for iterators, e.g., "step by index" or "step by value"
        public const string Sum = "sum"; // Used for math "the sum of n and m"
        public const string Difference = "difference"; // Used for math "the difference of n and m"
        public const string Product = "product"; // Used for math "the product of n and m"
        public const string Quotient = "quotient"; // Used for math "the quotient of n and m"
        public const string Modulo = "modulo"; // Used for math, eg "n moldulo 2"
        public const string Modulus = "%"; // Used for math, e.g., "n mod 2" (alternative to Modulo)
        public const string Raised = "raised"; // Used for math, e.g., "n raised to the power of 2"
        public const string Power = "power"; // Used for math, e.g., "n raised to the power of 2"
        public const string Type = "type"; // Used for type checking and casting
        public const string Shifted = "shifted"; // Used for bitwise operations, e.g., "n shifted left by 2" or "n shifted right by 2"
        public const string Left = "left"; // Used for bitwise operations, e.g., "n shifted left by 2"
        public const string Right = "right";
        public const string Bitwise = "bitwise"; // Used for bitwise operations, e.g., "n bitwise and m" or "n bitwise or m"
        public const string Rotated = "rotated"; // Used for bitwise rotation operations, e.g., "n rotated left by 2" or "n rotated right by 2"
        public const string Concatenated = "concatenated"; // Used for string concatenation, e.g., "n concatenated with m" or "n + m"
        public const string Otherwise = "otherwise"; // Used for default cases in conditional expressions, e.g., "if n is greater than m then ... otherwise ..."
        public const string Return = "return"; // Used for returning values from functions, e.g., "return the result of the function"
        public const string Label = "label"; // Used for labels in code, e.g., "label myLabel"
        public const string Goto = "goto"; // Used for jumping to labels in code, e.g., "goto myLabel"
        public const string Assert = "assert"; // Used for assertions in code, e.g., "assert condition is true"
        public const string Address = "address";
        public const string Size = "size";
        public const string Safe = "safe"; // Used for safe operations, e.g., "safe access to the array" or "safe dereference of pointer"
        public const string Unsafe = "unsafe"; // Used for unsafe operations, e.g., "unsafe access to the array" or "unsafe dereference of pointer"
        public const string Cast = "cast";
        public const string Break = "break"; // Used for breaking out of loops or switch statements
        public const string Continue = "continue"; // Used for skipping to the next iteration of loops

        public const string Pointer = "pointer"; // Used for pointer types, e.g., "int* myPointer" or "string* myStringPointer"
        public const string Constant = "constant";
        public const string Static = "static";
        public const string Nullable = "nullable"; // Used for nullable types, e.g., "int? myVariable" or "string? myString"
        public const string Returning = "returning";
        public const string Out = "out";
        public const string Restrict = "restrict";
        public const string Switch = "switch";
        public const string Match = "match";
        public const string Loop = "loop";


        public const string Increment = "increment";
        public const string Decrement = "decrement";
        public const string Set = "set"; // Used for assigning values, e.g., "set variable to value" or "set item to value"
        public const string Using = "using"; // Used for resource management, e.g., "using resource" or "using variable in scope"
        public const string Alias = "alias"; // Used for creating aliases, e.g., "alias myAlias for myVariable" or "alias myFunction for anotherFunction"
        public const string Import = "import"; // Used for importing modules or namespaces, e.g., "import myModule" or "import myNamespace"

        public const string Iterate = "iterate"; // Used for iterating over collections, e.g., "iterate over the elements of the array"
        public const string Over = "over"; // Used for specifying the collection to iterate over, e.g., "iterate over the elements of the array"
        public const string Execute = "execute"; // Used for executing commands or functions, e.g., "execute myCommand with parameters"
        public const string Following = "following"; // Used for specifying the next item in a sequence, e.g., "the following item in the list"
        public const string Fin = "fin"; // Used for indicating the end of a sequence or operation, e.g., "the fin of the list" or "the fin of the operation"

        public const string ListStart = ";";
        public const string CommaSeparator = ",";
        public const string EndOfStatement = ".";
        public const string OpenParenthesis = "(";
        public const string CloseParenthesis = ")";
        public const string Apostrophe = "'";
        public const string S = "s";
        public const string Wildcard = "_";
        #endregion

        private EnglishSymbolMatcher SymbolMatcher = new();

        public bool TryMatchKeyword(string raw, out TokenType type)
        {
            type = raw.Trim().ToLowerInvariant() switch
            {
                True => TokenType.LiteralTrue,
                False => TokenType.LiteralFalse,
                Null => TokenType.LiteralNull,
                Program => TokenType.Program,
                Loop => TokenType.Loop,

                #region DIGITS
                First => TokenType.LiteralPosition,
                Second => TokenType.LiteralPosition,
                Third => TokenType.LiteralPosition,
                Fourth => TokenType.LiteralPosition,
                Fifth => TokenType.LiteralPosition,
                Sixth => TokenType.LiteralPosition,
                Seventh => TokenType.LiteralPosition,
                Eighth => TokenType.LiteralPosition,
                Ninth => TokenType.LiteralPosition,
                Tenth => TokenType.LiteralPosition,
                Eleventh => TokenType.LiteralPosition,
                Twelfth => TokenType.LiteralPosition,
                Thirteenth => TokenType.LiteralPosition,
                Fourteenth => TokenType.LiteralPosition,
                Fifteenth => TokenType.LiteralPosition,
                Sixteenth => TokenType.LiteralPosition,
                Seventeenth => TokenType.LiteralPosition,
                Eighteenth => TokenType.LiteralPosition,
                Nineteenth => TokenType.LiteralPosition,
                Twentieth => TokenType.LiteralPosition,
                Thirtieth => TokenType.LiteralPosition,
                Fortieth => TokenType.LiteralPosition,
                Fiftieth => TokenType.LiteralPosition,
                Sixtieth => TokenType.LiteralPosition,
                Seventieth => TokenType.LiteralPosition,
                Eightieth => TokenType.LiteralPosition,
                Ninetieth => TokenType.LiteralPosition,
                Hundredth => TokenType.LiteralPosition,
                Thousandth => TokenType.LiteralPosition,
                Millionth => TokenType.LiteralPosition,
                Billionth => TokenType.LiteralPosition,
                Trillionth => TokenType.LiteralPosition,
                Quadrillionth => TokenType.LiteralPosition,
                Quintillionth => TokenType.LiteralPosition,
                // These are the string representations of numbers from 0 to 20 and multiples of ten up to ninety.
                One => TokenType.LiteralNumber,
                Two => TokenType.LiteralNumber,
                Three => TokenType.LiteralNumber,
                Four => TokenType.LiteralNumber,
                Five => TokenType.LiteralNumber,
                Six => TokenType.LiteralNumber,
                Seven => TokenType.LiteralNumber,
                Eight => TokenType.LiteralNumber,
                Nine => TokenType.LiteralNumber,
                Zero => TokenType.LiteralNumber,
                Ten => TokenType.LiteralNumber,
                Eleven => TokenType.LiteralNumber,
                Twelve => TokenType.LiteralNumber,
                Thirteen => TokenType.LiteralNumber,
                Fourteen => TokenType.LiteralNumber,
                Fifteen => TokenType.LiteralNumber,
                Sixteen => TokenType.LiteralNumber,
                Seventeen => TokenType.LiteralNumber,
                Eighteen => TokenType.LiteralNumber,
                Nineteen => TokenType.LiteralNumber,
                Twenty => TokenType.LiteralNumber,
                Thirty => TokenType.LiteralNumber,
                Forty => TokenType.LiteralNumber,
                Fifty => TokenType.LiteralNumber,
                Sixty => TokenType.LiteralNumber,
                Seventy => TokenType.LiteralNumber,
                Eighty => TokenType.LiteralNumber,
                Ninety => TokenType.LiteralNumber,
                Hundred => TokenType.LiteralNumber,
                Thousand => TokenType.LiteralNumber,
                Million => TokenType.LiteralNumber,
                Billion => TokenType.LiteralNumber,
                Trillion => TokenType.LiteralNumber,
                Quadrillion => TokenType.LiteralNumber,
                Quintillion => TokenType.LiteralNumber,
                //Sextillion => TokenType.LiteralNumber,
                //Septillion => TokenType.LiteralNumber,
                //Octillion => TokenType.LiteralNumber,
                //Nonillion => TokenType.LiteralNumber,
                //Decillion => TokenType.LiteralNumber,
                //Undecillion => TokenType.LiteralNumber,
                //Duodecillion => TokenType.LiteralNumber,
                //Tredecillion => TokenType.LiteralNumber,
                //Quattuordecillion => TokenType.LiteralNumber,
                //Quindecillion => TokenType.LiteralNumber,
                //Sexdecillion => TokenType.LiteralNumber,
                //Septendecillion => TokenType.LiteralNumber,
                //Octodecillion => TokenType.LiteralNumber,
                //Novemdecillion => TokenType.LiteralNumber,
                //Vigintillion => TokenType.LiteralNumber,
                //Unvigintillion => TokenType.LiteralNumber,
                //Duovigintillion => TokenType.LiteralNumber,
                //Trevigintillion => TokenType.LiteralNumber,
                Point => TokenType.LiteralNumber,
                Negative => TokenType.LiteralNumber,
                #endregion


                Pointer => TokenType.KeywordPointer, // Used for pointer types, e.g., "int* myPointer" or "string* myStringPointer"
                Constant => TokenType.KeywordConstant, // Used for constant values, e.g., "constant myConstant = 42"
                Static => TokenType.KeywordStatic, // Used for static variables or methods, e.g., "static int myStaticVariable" or "static void MyStaticMethod()"
                Nullable => TokenType.KeywordNullable, // Used for nullable types, e.g., "int? myVariable" or "string? myString"

                TypeEnum => TokenType.TypeEnum,
                Switch => TokenType.KeywordSwitch,
                Match => TokenType.KeywordMatch,
                Case => TokenType.KeywordCase,
                Define => TokenType.KeywordDefine,
                Is => TokenType.KeywordIs,
                Index => TokenType.KeywordIndex, // Used for array access and is value keyword in Carmen iterators
                A => TokenType.KeywordA,
                An => TokenType.KeywordA,
                And => TokenType.KeywordAnd,
                As => TokenType.KeywordAs,
                Equal => TokenType.KeywordEqual,
                Of => TokenType.KeywordOf,
                To => TokenType.KeywordTo,
                From => TokenType.KeywordFrom,
                Last => TokenType.KeywordLast, // Used for iterators
                The => TokenType.KeywordThe, // Used for iterators, e.g., "the last item"
                Item => TokenType.KeywordItem, // Used for indexers, e.g., "the last item" & used in iterators item.Value = "item value"; within iterators
                Final => TokenType.KeywordFinal, // Used for iterators, e.g., "the final item"
                Initial => TokenType.KeywordInitial, // Used for iterators, e.g., "the initial item"
                Sum => TokenType.KeywordSum, // Used for math "the sum of n and m"
                Difference => TokenType.KeywordDifference, // Used for math "the difference of n and m"
                Product => TokenType.KeywordProduct, // Used for math "the product of n and m"
                Quotient => TokenType.KeywordQuotient, // Used for math "the quotient of n and m"
                Modulo => TokenType.KeywordModulo, // Used for math, eg "n moldulo 2"
                Modulus => TokenType.KeywordModulo, // Used for math, e.g., "n mod 2" (alternative to Modulo)
                Raised => TokenType.KeywordRaised, // Used for math, e.g., "n raised to the power of 2"
                Power => TokenType.KeywordPower, // Used for math, e.g., "n raised to the power of 2"
                Type => TokenType.KeywordType, // Used for type checking and casting
                Not => TokenType.KeywordNot, // Used for boolean negation, e.g., "not true" or "not false"
                Or => TokenType.KeywordOr, // Used for boolean operations, e.g., "true or false"
                Xor => TokenType.OperatorXor, // Used for boolean exclusive or, e.g., "true xor false"
                LogicalXOR => TokenType.OperatorXor, // Used for boolean exclusive or, e.g., "true ⊕ false"
                Exclusive => TokenType.KeywordExclusive, // Used for boolean exclusive or, e.g., "true exclusive ..or.. false"
                Greater => TokenType.KeywordGreater, // Used for comparison, e.g., "n greater than m"
                Less => TokenType.KeywordLess, // Used for comparison, e.g., "n less than m"
                Than => TokenType.KeywordThan, // Used for comparison, e.g., "n greater than m" or "n less than m"
                Increment => TokenType.OperatorIncrement,
                Decrement => TokenType.OperatorDecrement,
                By => TokenType.KeywordBy, // Used for iterators, e.g., "by index" or "by value"
                Shifted => TokenType.KeywordShifted, // Used for bitwise operations, e.g., "n shifted left by 2" or "n shifted right by 2"
                Left => TokenType.KeywordLeft, // Used for bitwise operations, e.g., "n shifted left by 2"
                Right => TokenType.KeywordRight, // Used for bitwise operations, e.g., "n shifted right by 2"
                Bitwise => TokenType.KeywordBitwise, // Used for bitwise operations, e.g., "n bitwise and m" or "n bitwise or m"
                Rotated => TokenType.KeywordRotated, // Used for bitwise rotation operations, e.g., "n rotated left by 2" or "n rotated right by 2"
                If => TokenType.KeywordIf, // Used for conditional expressions, e.g., "if n is greater than m then ..."
                Then => TokenType.KeywordThen, // Used for conditional expressions, e.g., "if n is greater than m then ..."
                Call => TokenType.KeywordCall, // Used for function calls, e.g., "call myFunction with parameters"
                With => TokenType.KeywordWith, // Used for function calls, e.g., "call myFunction with parameters"
                Concatenated => TokenType.KeywordConcatenated, // Used for string concatenation, e.g., "n concatenated with m" or "n + m"
                Otherwise => TokenType.KeywordOtherwise, // Used for default cases in conditional expressions, e.g., "if n is greater than m then ... otherwise ..."
                Elements => TokenType.KeywordElements, // Used for iterators, e.g., "the elements of the array" or "the elements of the list"
                At => TokenType.KeywordAt, // Used for accessing elements at a specific index, e.g., "the element at index 0 of the array"
                Through => TokenType.KeywordThrough, // Used for iterators, e.g., "the elements of the array through index 5"
                Beginning => TokenType.KeywordBeginning, // Used for iterators, e.g., "the beginning of the array" or "the first element of the array"
                Ending => TokenType.KeywordEnding, // Used for iterators, e.g., "the ending of the array" or "the last element of the array"

                Set => TokenType.KeywordSet, // Used for assigning values, e.g., "set variable to value" or "set item to value"
                Using => TokenType.KeywordUsing, // Used for resource management, e.g., "using resource" or "using variable in scope"
                Alias => TokenType.KeywordAlias, // Used for creating aliases, e.g., "alias myAlias for myVariable" or "alias myFunction for anotherFunction"
                Import => TokenType.KeywordImport, // Used for importing modules or namespaces, e.g., "import myModule" or "import myNamespace"

                Safe => TokenType.KeywordSafe, // Used for safe operations, e.g., "safe access to the array" or "safe dereference of pointer"
                Unsafe => TokenType.KeywordUnsafe, // Used for unsafe operations, e.g., "unsafe access to the array" or "unsafe dereference of pointer"
                Cast => TokenType.KeywordCast, // Used for type casting, e.g., "cast variable to type" or "cast value to type"

                Out => TokenType.KeywordOut,
                Restrict => TokenType.KeywordRestrict,

                Break => TokenType.KeywordBreak, // Used for breaking out of loops or switch statements
                Continue => TokenType.KeywordContinue, // Used for skipping to the next iteration of loops

                Value => TokenType.KeywordValue, // Used for accessing the value of an item, e.g., "item.Value = 'item value';" within iterators
                Apostrophe => TokenType.PunctuationApostrophe, // Used for character literals, e.g., 'a' or 'b'
                S => TokenType.PunctuationS, // Used for pluralization, e.g., "items" or "elements"
                Return => TokenType.KeywordReturn, // Used for returning values from functions, e.g., "return the result of the function"

                Iterate => TokenType.KeywordIterate, // Used for iterating over collections, e.g., "iterate over the elements of the array"
                Over => TokenType.KeywordOver, // Used for specifying the collection to iterate over, e.g., "iterate over the elements of the array"

                Step => TokenType.KeywordStep, // Used for iterators, e.g., "step by index" or "step by value"
                Returning => TokenType.KeywordReturning,

                Label => TokenType.KeywordLabel, // Used for labels in code, e.g., "label myLabel"
                Goto => TokenType.KeywordGoto, // Used for jumping to labels in code, e.g., "goto myLabel"
                Assert => TokenType.KeywordAssert, // Used for assertions in code, e.g., "assert condition is true"
                Address => TokenType.KeywordAddress, // Used for memory address operations, e.g., "address of variable"
                Size => TokenType.KeywordSize, // Used for getting the size of a data structure or type, e.g., "size of array" or "size of type"

                Execute => TokenType.KeywordExecute, // Used for executing commands or functions, e.g., "execute myCommand with parameters"
                Following => TokenType.KeywordFollowing, // Used for specifying the next item in a sequence, e.g., "the following item in the list"
                Fin => TokenType.KeywordFin, // Used for indicating the end of a sequence or operation, e.g., "the fin of the list" or "the fin of the operation"
                Each => TokenType.KeywordEach, // Used for iterators, e.g., "each item in the array" or "each element in the list"

                #region TYPE SYMBOLS
                TypeString => TokenType.TypeString,
                TypeString + "s" => TokenType.TypeString, // Allow plural form for consistency
                TypeBoolean => TokenType.TypeBoolean,
                TypeBoolean + "s" => TokenType.TypeBoolean, // Allow plural form for consistency
                TypeByte => TokenType.TypeByte,
                TypeByte + "s" => TokenType.TypeByte, // Allow plural form for consistency
                TypeChar => TokenType.TypeChar,
                TypeChar + "s" => TokenType.TypeChar, // Allow plural form for consistency
                TypeShort => TokenType.TypeShort,
                TypeShort + "s" => TokenType.TypeShort, // Allow plural form for consistency
                TypeInt => TokenType.TypeInt,
                TypeInt + "s" => TokenType.TypeInt, // Allow plural form for consistency
                TypeLong => TokenType.TypeLong,
                TypeLong + "s" => TokenType.TypeLong, // Allow plural form for consistency
                TypeFloat => TokenType.TypeFloat,
                TypeFloat + "s" => TokenType.TypeFloat, // Allow plural form for consistency
                TypeDouble => TokenType.TypeDouble,
                TypeDouble + "s" => TokenType.TypeDouble, // Allow plural form for consistency
                TypeDecimal => TokenType.TypeDecimal,
                TypeDecimal + "s" => TokenType.TypeDecimal, // Allow plural form for consistency
                TypeObject => TokenType.TypeObject,
                TypeObject + "s" => TokenType.TypeObject, // Allow plural form for consistency

                TypeStructure => TokenType.TypeStruct,
                #endregion

                ListStart => TokenType.PunctuationSemicolon,
                CommaSeparator => TokenType.PunctuationComma,
                EndOfStatement => TokenType.EndOfStatement,
                OpenParenthesis => TokenType.PunctuationOpenParenthesis,
                CloseParenthesis => TokenType.PunctuationCloseParenthesis,

                FunctionIdentifier => TokenType.FuncIdentifier,
                VariableIdentifier => TokenType.VariableIdentifier,
                StructurIdentifier => TokenType.StructIdentifier,
                LabelIdentifier => TokenType.LabelIdentifier,
                AliasIdentifier => TokenType.AliasIdentifier,

                _ => TokenType.Unknown // Not A Keyword
            };
            return type != TokenType.Unknown;
        }

        public bool TryMatchComplex(string raw, out TokenType type)
        {
            if (raw.StartsWith(SymbolMatcher.StringLiteralChar) && 
                raw.EndsWith(SymbolMatcher.StringLiteralChar)) 
            {
                type = TokenType.LiteralString;
                return true;
            }
            else if (raw.StartsWith(SymbolMatcher.CharLiteralChar) && 
                raw.EndsWith(SymbolMatcher.CharLiteralChar))
            {
                type = TokenType.LiteralCharacter;
                return true;
            }
            else if (raw.Length > 2 &&
                raw[0] == SymbolMatcher.CharZero &&
                (raw[1] == SymbolMatcher.HexPrefix || raw[1] == SymbolMatcher.BinaryPrefix))
            {
                type = TokenType.LiteralNumber;
                return true;
            }
            else if (decimal.TryParse(raw, out _))
            {
                type = TokenType.LiteralNumber;
                return true;
            }
            type = TokenType.Unknown;
            return false;
        }

        public bool TryConvertToDecimal(Token token, out decimal value) 
        {
            value = 0;
            if (decimal.TryParse(token.Raw, out value))
                return true;
            if (!token.IsNumeric())
                return false;
            value = token.Raw switch
            {
                Zero => 0,
                One => 1,
                Two => 2,
                Three => 3,
                Four => 4,
                Five => 5,
                Six => 6,
                Seven => 7,
                Eight => 8,
                Nine => 9,
                Ten => 10,
                Eleven => 11,
                Twelve => 12,
                Thirteen => 13,
                Fourteen => 14,
                Fifteen => 15,
                Sixteen => 16,
                Seventeen => 17,
                Eighteen => 18,
                Nineteen => 19,
                Twenty => 20,
                Thirty => 30,
                Forty => 40,
                Fifty => 50,
                Sixty => 60,
                Seventy => 70,
                Eighty => 80,
                Ninety => 90,
                Hundred => 100,
                Thousand => 1000,
                Million => 1000000,
                Billion => 1000000000,
                Trillion => 1000000000000,
                Quadrillion => 1000000000000000,
                Quintillion => 1000000000000000000,

                First => 1,
                Second => 2,
                Third => 3,
                Fourth => 4,
                Fifth => 5,
                Sixth => 6,
                Seventh => 7,
                Eighth => 8,
                Ninth => 9,
                Tenth => 10,
                Eleventh => 11,
                Twelfth => 12,
                Thirteenth => 13,
                Fourteenth => 14,
                Fifteenth => 15,
                Sixteenth => 16,
                Seventeenth => 17,
                Eighteenth => 18,
                Nineteenth => 19,
                Twentieth => 20,
                Thirtieth => 30,
                Fortieth => 40,
                Fiftieth => 50,
                Sixtieth => 60,
                Seventieth => 70,
                Eightieth => 80,
                Ninetieth => 90,
                Hundredth => 100,
                Thousandth => 1000,
                Millionth => 1000000,
                Billionth => 1000000000,
                Trillionth => 1000000000000,
                Quadrillionth => 1000000000000000,
                Quintillionth => 1000000000000000000,
                Point => 0.0m, // Decimal point as a fraction.
                _ => throw new NotImplementedException($"Unknown numeric token: {token.Raw} at line {token.Line}, column {token.Column}.")
            };
            return true;
        }

        public bool TryConvertToDecimal(Token[] tokens, out decimal value)
        {
            value = default;
            if (!tokens.IsNumeric()) return false;
            if (tokens.Length == 1) return TryConvertToDecimal(tokens[0], out value);
            
            bool inDecimal = false;
            decimal multiplier = 1;

            for (var i = 0l ; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (!TryConvertToDecimal(token, out var digit)) return false;
                if (token.Raw == Point) // String form decimal point
                {
                    if (inDecimal)
                    {
                        value = 0;
                        return false;
                    }
                    inDecimal = true;
                    multiplier = 1;
                    continue;
                }
                if (inDecimal) 
                {
                    multiplier *= 0.1m;
                }
                switch (token.Raw)
                {
                    case Hundred:
                    case Hundredth:
                    case Thousand:
                    case Thousandth:
                    case Million:
                    case Millionth:
                    case Billion:
                    case Billionth:
                    case Trillion:
                    case Trillionth:
                    case Quadrillion:
                    case Quadrillionth:
                    case Quintillion:
                    case Quintillionth:
                        if (value == 0) value = 1;
                        value *= digit;
                        break;
                    default:
                        value += digit * multiplier;
                        break;
                }
            }
            return true;
        }
    }
}
