using Arcane.Carmen.AST.Types;
using Arcane.Carmen.Lexer;
using System.Reflection;
using System.Xml.Linq;

namespace Arcane.Carmen.Parser;

public enum Keywords
{
    Unknown,
    Program,
    Define,
    Decrement,
    Increment,
    Goto,
    Label,
    Is,
    Not,
    A,
    By,
    Or,
    The,
    Index,

    And,
    Xor,

    Sum,
    Difference,
    Product,
    Quotient,
    Modulo,
    Raised,
    Power,
    Square,
    Root,

    Of,
    Type,
    OfType,

    With,
    Size,
    WithSize,

    If,
    Then,

    TheAddressOf,
    Address,
    Assign,
    Set,
    Equal,
    To,
    EqualTo,

    Less, Than, Greater,
    IsEqualTo,
    IsNotEqualTo,
    IsLessThan,
    IsLessThanOrEqualTo,
    IsGreaterThan,
    IsGreaterThanOrEqualTo,
    
    True,
    False,

    IsA,
    IsNotA,

    Put,
    Into,

    Null,

    Constant,
    Static,

    Nullable,
    Pointer,
    PointerTo,

    Semicolon,
    Comma,
    EOS,

    BlockStart,
    BlockEnd,

    OpenParen,
    CloseParen,

    TheSumOf,
    TheDifferenceOf,
    TheProductOf,
    TheQuotientOf,
    TheModuloOf,
    RaisedToThePowerOf,
    RootOf,

    Bitwise,
    BitwiseOr,
    BitwiseAnd,
    BitwiseXor,
    BitwiseNot,

    Divided,

    Adder, // + 'plus'
    Subtractor, // - 'minus'
    Multiplier, // * 'times'
    Divider, // / 'divided by'

    Shifted,
    Rotated,
    Left, 
    Right,

    ShiftedLeftBy,
    ShiftedRightBy,
    RotatedLeftBy,
    RotatedRightBy,

    Squared,
    Cubed,
    Cubic,

    Otherwise,
    IfNotNullOtherwise,

    Concatenated,
    ConcatenatedWith,

    Apostrophe,
    S,
    ApostrophyS,

    Elements,
    From,
    Until,
    Every,
    Element,
    ElementOf,
}

public static class KeywordsEx
{
    public static bool TryMatchKeyword(this Token keyword, out Keywords word)
    {
        word = keyword.Content switch
        {
            "program" or "𐑐𐑮𐑴𐑜𐑮𐑨𐑥" => Keywords.Program,
            "define" or "𐑛𐑳𐑓𐑲𐑯" => Keywords.Define,

            "a" or "an" or "𐑩" or "𐑩𐑯" => Keywords.A,
            "is" or "𐑦𐑕" => Keywords.Is,
            "by" or "𐑚𐑲" => Keywords.By,
            "not" or "𐑯𐑴𐑑" => Keywords.Not,
            "or" or "𐑹" => Keywords.Or,
            "and" or "&&" or "𐑨𐑯𐑛" => Keywords.And,
            "xor" or "^" or "𐑟𐑹" => Keywords.Xor,

            "the" or "𐑞𐑳" => Keywords.Or,
            "address" or "𐑨𐑛𐑮𐑧𐑕" => Keywords.Address,
            
            "of" or "𐑳𐑓" => Keywords.Of,
            "type" or "𐑑𐑲𐑐" => Keywords.Type,
            "with" or "𐑘𐑦𐑔" => Keywords.With,
            "size" or "𐑕𐑲𐑟" => Keywords.Size,

            "put" or "𐑐𐑳𐑑" => Keywords.Put,
            "into" or "𐑦𐑯𐑑𐑵" => Keywords.Into,

            "if" or "𐑦𐑓" => Keywords.If,
            "then" or "𐑞𐑧𐑯" => Keywords.Then,

            "goto" or "𐑜𐑴𐑑𐑵" => Keywords.Goto,
            "label" or "𐑤𐑱𐑚𐑤" => Keywords.Label,

            "index" or "𐑦𐑯𐑛𐑧𐑒𐑕" => Keywords.Index,

            "true" or "𐑑𐑮𐑵" => Keywords.True,
            "false" or "𐑓𐑩𐑤𐑕" => Keywords.False,

            "null" or "𐑯𐑳𐑤" => Keywords.Null,

            "nullable" or "𐑯𐑳𐑤𐑩𐑚𐑤" => Keywords.Nullable,
            "pointer" or "𐑐𐑶𐑯𐑑𐑮" => Keywords.Pointer,

            "set" or "𐑕𐑧𐑑" => Keywords.Set,
            "assign" or "𐑩𐑕𐑲𐑯" => Keywords.Assign,
            "equal" or "𐑰𐑒𐑢𐑩𐑤" => Keywords.Equal,
            "to" or "𐑑𐑵" => Keywords.To,

            "sb" or "𐑕𐑚" or ":" or "{" => Keywords.BlockStart,
            "eb" or "𐑧𐑚" or "𐑓𐑦𐑯" or "}" or "…" => Keywords.BlockEnd,

            "op" or "𐑴𐑐" or "(" => Keywords.OpenParen,
            "ep" or "𐑧𐑐" or ")" => Keywords.CloseParen,

            "less" or "𐑤𐑧𐑕" => Keywords.Less,
            "greater" or "𐑜𐑮𐑱𐑑𐑮" => Keywords.Greater,
            "than" or "𐑞𐑩𐑯" => Keywords.Than,
            "=" => Keywords.EqualTo,
            "==" => Keywords.IsEqualTo,
            "!=" => Keywords.IsNotEqualTo,
            "<" or "‹" => Keywords.IsLessThan,
            "<=" or "‹=" => Keywords.IsLessThanOrEqualTo,
            ">" or "›" => Keywords.IsGreaterThan,
            ">=" or "›=" => Keywords.IsGreaterThanOrEqualTo,

            "constant" or "𐑒𐑪𐑯𐑕𐑑𐑩𐑯𐑑" => Keywords.Constant,
            "static" or "𐑕𐑑𐑨𐑑𐑦𐑒" => Keywords.Static,

            "increment" or "𐑦𐑯𐑒𐑮𐑧𐑥𐑧𐑯𐑑" => Keywords.Increment,
            "decrement" or "𐑛𐑧𐑒𐑮𐑧𐑥𐑧𐑯𐑑" => Keywords.Decrement,

            ";" => Keywords.Semicolon,
            "." => Keywords.EOS,
            "," => Keywords.Comma,

            "sum" or "𐑕𐑳𐑥" => Keywords.Sum,
            "difference" or "𐑛𐑦𐑓𐑮𐑩𐑯𐑕" => Keywords.Difference,
            "product" or "𐑐𐑮𐑭𐑛𐑳𐑒𐑑" => Keywords.Product,
            "quotient" or "𐑒𐑘𐑴𐑖𐑧𐑯𐑑" => Keywords.Quotient,
            "modulo" or "𐑥𐑪𐑛𐑢𐑤𐑴" or
            "modulus" or "𐑥𐑪𐑛𐑢𐑤𐑳𐑕" or "%" => Keywords.Modulo,
            "raised" or "𐑮𐑱𐑟𐑛" => Keywords.Raised,
            "power" or "𐑐𐑶𐑮" => Keywords.Power,

            "squared" or "𐑕𐑒𐑢𐑺𐑛" => Keywords.Squared,
            "cubed" or "𐑒𐑿𐑚𐑛" => Keywords.Cubed,

            "square" or "𐑕𐑒𐑢𐑺" => Keywords.Square,
            "cubic" or "𐑒𐑿𐑚𐑦𐑒" => Keywords.Cubic,

            "root" or "𐑮𐑵𐑑" => Keywords.Root,
            "√" => Keywords.RootOf, // ALT+251

            "+" or "plus" or "𐑐𐑤𐑳𐑕" => Keywords.Adder,
            "-" or "minus" or "𐑥𐑲𐑯𐑳𐑕" => Keywords.Subtractor,
            "*" or "times" or "𐑑𐑲𐑤𐑟" => Keywords.Multiplier,
            "/" => Keywords.Divider,

            "divided" or "𐑛𐑦𐑝𐑲𐑛𐑧𐑛" => Keywords.Divided,

            "bitwise" or "𐑚𐑦𐑑𐑢𐑲𐑟" => Keywords.Bitwise,

            "shifted" => Keywords.Shifted,
            "rotated" => Keywords.Rotated,
            "left" => Keywords.Left,
            "right" => Keywords.Right,

            "<<" => Keywords.ShiftedLeftBy,
            ">>" => Keywords.ShiftedRightBy,

            "ROL" => Keywords.RotatedLeftBy,
            "ROR" => Keywords.RotatedRightBy,

            "otherwise" or "𐑳𐑞𐑮𐑢𐑲𐑟" => Keywords.Otherwise,
            "??" => Keywords.IfNotNullOtherwise,

            "concatenated" or "𐑒𐑳𐑯𐑒𐑨𐑑𐑧𐑯𐑱𐑑𐑛" => Keywords.Concatenated,

            "'" => Keywords.Apostrophe,
            "s" or "𐑕" => Keywords.S,

            "elements" or "𐑧𐑤𐑳𐑥𐑧𐑯𐑑𐑕" => Keywords.Elements,
            "from" or "𐑓𐑮𐑳𐑥" => Keywords.From,
            "until" or "𐑳𐑯𐑑𐑦𐑤" => Keywords.Until,
            "every" or "𐑧𐑝𐑧𐑮𐑰" => Keywords.Every,
            "element" or "𐑧𐑤𐑳𐑥𐑧𐑯𐑑" => Keywords.Element,

            _ => Keywords.Unknown
        };
        return word != Keywords.Unknown;
    }

    public static bool TryMatchBaseType(this Token typeword, out Primitives word)
    {
        word = typeword.Content switch
        {
            "byte" or "𐑚𐑲𐑑" or
            "bytes" or "𐑚𐑲𐑑𐑕" => Primitives.Byte,
            "sbyte" or "𐑕𐑚𐑲𐑑" or
            "sbytes" or "𐑕𐑚𐑲𐑑𐑕" => Primitives.SByte,
            "short" or "𐑖𐑹𐑑" or
            "shorts" or "𐑖𐑹𐑑𐑕" => Primitives.Short,
            "ushort" or "𐑢𐑖𐑹𐑑" or
            "ushorts" or "𐑢𐑖𐑹𐑑𐑕" => Primitives.UShort,
            "integer" or "𐑦𐑯𐑑𐑧𐑡𐑮" or
            "integers" or "𐑦𐑯𐑑𐑧𐑡𐑮𐑟" => Primitives.Integer,
            "uinteger" or "𐑢𐑦𐑯𐑑𐑧𐑡𐑮" or
            "integers" or "𐑢𐑦𐑯𐑑𐑧𐑡𐑮𐑟" => Primitives.UInteger,
            "long" or "𐑤𐑪𐑙" or
            "longs" or "𐑤𐑪𐑙𐑕" => Primitives.Long,
            "ulong" or "𐑢𐑤𐑪𐑙" or
            "ulongs" or "𐑢𐑤𐑪𐑙𐑕" => Primitives.ULong,
            "single" or "float" or "𐑕𐑦𐑯𐑜𐑮" or
            "singles" or "floats" or "𐑕𐑦𐑯𐑜𐑮𐑕" => Primitives.Float,
            "double" or "𐑛𐑳𐑚𐑤" or
            "doubles" or "𐑛𐑳𐑚𐑤𐑕" => Primitives.Double,
            "decimal" or "𐑛𐑧𐑕𐑦𐑥𐑩𐑤" or
            "decimals" or "𐑛𐑧𐑕𐑦𐑥𐑩𐑤𐑕" => Primitives.Decimal,
            "void" or "𐑝𐑶𐑛" or 
            "voids" or "𐑝𐑶𐑛𐑕" => Primitives.Void,
            "string" or "𐑕𐑑𐑮𐑦𐑙" or
            "strings" or "𐑕𐑑𐑮𐑦𐑙𐑕" => Primitives.String,
            "array" or "𐑼𐑱" or
            "arrays" or "𐑼𐑱𐑕" => Primitives.Array,
            _ => Primitives.NotBase
        }; 
        return word != Primitives.NotBase;
    }
}
