namespace Arcane.Carmen.Lexer.Tokens
{

    public static class TokenTypeExtensions
    {
        public static bool IsLiteral(this TokenType type)
        {
            return type switch
            {
                TokenType.LiteralString or
                TokenType.LiteralNumber or
                TokenType.LiteralPosition or
                TokenType.LiteralTrue or
                TokenType.LiteralFalse or
                TokenType.LiteralNull or
                TokenType.LiteralCharacter => true,
                _ => false,
            };
        }

        public static bool IsType(this TokenType type)
        {
            return type switch
            {
                TokenType.StructIdentifier or
                TokenType.TypeString or
                TokenType.TypeBoolean or
                TokenType.TypeByte or
                TokenType.TypeChar or
                TokenType.TypeShort or
                TokenType.TypeInt or
                TokenType.TypeLong or
                TokenType.TypeFloat or
                TokenType.TypeDouble or
                TokenType.TypeDecimal or
                TokenType.TypeObject or
                TokenType.TypeStruct => true,
                _ => false,
            };
        }

        public static bool IsIdentifier (this TokenType type)
        {
            return type switch
            {
                TokenType.VariableIdentifier or
                TokenType.FuncIdentifier or
                TokenType.LabelIdentifier or
                TokenType.AliasIdentifier or
                TokenType.StructIdentifier => true,
                _ => false,
            };
        }
    }

    public enum TokenType
    {
        Unknown = -1,
        // Value Symbols
        LiteralString,
        LiteralNumber,
        LiteralPosition,
        LiteralTrue,
        LiteralFalse,
        LiteralNull,
        VariableIdentifier,
        FuncIdentifier,
        LabelIdentifier,
        AliasIdentifier,
        LiteralCharacter,
        // Type Symbols
        StructIdentifier,
        TypeString,
        TypeBoolean,
        TypeByte,
        TypeChar,
        TypeShort,
        TypeInt,
        TypeLong,
        TypeFloat,
        TypeDouble,
        TypeDecimal,
        TypeObject,
        TypeStruct,

        // Punctuation

        // Keywords
        KeywordDefine,
        KeywordIs,
        KeywordA,
        KeywordAnd,
        KeywordAs,
        KeywordEqual,
        KeywordOf,
        KeywordArray,
        KeywordTo,
        KeywordIndex,
        KeywordFrom,
        KeywordLast,
        KeywordThe,
        KeywordItem,
        KeywordFinal,
        KeywordInitial,
        KeywordSum,
        KeywordDifference,
        KeywordProduct,
        KeywordQuotient,
        KeywordModulo,
        KeywordPower,
        KeywordRaised,
        KeywordType,
        KeywordNot,
        KeywordOr,
        OperatorXor,
        KeywordExclusive,
        KeywordGreater,
        KeywordLess,
        KeywordThan,
        KeywordBy,
        KeywordWith,
        KeywordLeft,
        KeywordRight,
        KeywordIn,
        KeywordBitwise,
        KeywordIf,
        KeywordThen,
        KeywordCall,
        KeywordOtherwise,
        KeywordElements,
        KeywordAt,
        KeywordThrough,
        KeywordBeginning,
        KeywordEnding,
        KeywordReturn,
        KeywordLabel,
        KeywordGoto,
        KeywordAssert,
        KeywordAddress,
        KeywordSize,
        KeywordSafe,
        KeywordUnsafe,
        KeywordCast,
        KeywordBreak,
        KeywordContinue,
        KeywordExecute,
        KeywordFollowing,
        KeywordFin,
        KeywordSet,
        KeywordAlias,
        KeywordUsing,
        KeywordImport,
        KeywordIterate,
        KeywordOver,
        KeywordValue,
        KeywordEach,
        KeywordStep,
        KeywordYield,
        KeywordReturning,

        KeywordPointer,
        KeywordConstant,
        KeywordStatic,
        KeywordNullable,

        KeywordWhile,
        KeywordFor,
        KeywordDo,
        KeywordUntil,

        KeywordSwitch,
        KeywordCase,
        KeywordDefault,

        KeywordFunction,
        KeywordStructure,

        KeywordOut,
        KeywordRestrict,



        // Keywords (Condensed)
        KeywordAsA,
        KeywordIsA,
        KeywordForEach,
        KeywordPointerTo,
        KeywordIterateOver,
        KeywordWithIndexAs,
        KeywordWithValueAs,
        KeywordEqualTo,
        KeywordCommaAnd,
        KeywordArrayOf,
        KeywordFromLast,
        KeywordOtherwiseIf,
        OperatorMathPow,
        KeywordTheSumOf,
        KeywordTheDifferenceOf,
        KeywordTheProductOf,
        KeywordTheQuotientOf,
        OperatorGreaterThan,
        OperatorLessThan,
        OperatorLessThanOrEqualTo,
        OperatorGreaterThanOrEqualTo,
        OperatorNullCoalesce,
        KeywordIsNull,
        KeywordIsNotNull,
        KeywordBeginningAt,
        KeywordEndingAt,
        KeywordOfSize,
        KeywordIsOfType,
        KeywordDefineFunction,
        KeywordDefineStructure,

        // Operators
        KeywordShifted,
        KeywordRotated,
        OperatorIncrement,
        OperatorDecrement,
        KeywordConcatenated,

        // Operators (Condensed)
        OperatorShiftedLeftBy,
        OperatorShiftedRightBy,
        OperatorRotatedLeftBy,
        OperatorRotatedRightBy,

        OperatorBitwiseAnd,
        OperatorBitwiseOr,
        OperatorBitwiseXor,
        OperatorBitwiseNot,

        OperatorConcatenatedWith,
        OperatorTheAddressOf,

        // Punctuation
        PunctuationColon,
        PunctuationSemicolon,
        PunctuationComma,
        PunctuationOpenParenthesis,
        PunctuationCloseParenthesis,

        PunctuationS,
        PunctuationApostrophe,

        OperatorMemberAccess,

        BlockStart,
        BlockEnd,

        EndOfStatement,
    }
}
