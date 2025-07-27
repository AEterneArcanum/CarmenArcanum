using Arcane.Carmen.AST.Types;
using Arcane.Carmen.Lexer;

namespace Arcane.Carmen.Parser;

public enum Keywords
{
    Unknown,
    Program,
    Decrement,
    Increment,
    Goto,
    Label,
    Is,
    A,

    If,
    Then,

    Set,
    Equal,
    To,

    IsEqualTo,
    IsNotEqualTo,
    IsLessThan,
    IsLessThanOrEqualTo,
    IsGreaterThan,
    IsGreaterThanOrEqualTo,
    
    True,
    False,

    Null,

    EOS,

    BlockStart,
    BlockEnd,

    OpenParen,
    CloseParen,

}

public static class KeywordsEx
{
    public static bool TryMatchKeyword(this Token keyword, out Keywords word)
    {
        word = keyword.Content switch
        {
            "program" or "𐑐𐑮𐑴𐑜𐑮𐑨𐑥" => Keywords.Program,

            "a" or "an" or "𐑩" or "𐑩𐑯" => Keywords.A,
            "is" or "𐑦𐑕" => Keywords.Is,

            "if" or "𐑦𐑓" => Keywords.If,
            "then" or "𐑞𐑧𐑯" => Keywords.Then,

            "goto" or "𐑜𐑴𐑑𐑵" => Keywords.Goto,
            "label" or "𐑤𐑱𐑚𐑤" => Keywords.Label,

            "true" or "𐑑𐑮𐑵" => Keywords.True,
            "false" or "𐑓𐑩𐑤𐑕" => Keywords.False,

            "null" or "𐑯𐑳𐑤" => Keywords.Null,

            "set" or "𐑕𐑧𐑑" => Keywords.Set,
            "equal" or "𐑰𐑒𐑢𐑩𐑤" => Keywords.Equal,
            "to" or "𐑑𐑵" => Keywords.To,

            "sb" or "𐑕𐑚" or ":" or "{" => Keywords.BlockStart,
            "eb" or "𐑧𐑚" or "𐑓𐑦𐑯" or "}" or "…" => Keywords.BlockEnd,

            "op" or "𐑴𐑐" or "(" => Keywords.OpenParen,
            "ep" or "𐑧𐑐" or ")" => Keywords.CloseParen,

            "isequalto" or "==" => Keywords.IsEqualTo,
            "isnotequalto" or "!=" => Keywords.IsNotEqualTo,
            "islessthan" or "<" or "‹" => Keywords.IsLessThan,
            "islessthanorequalto" or "<=" or "‹=" => Keywords.IsLessThanOrEqualTo,
            "isgreaterthan" or ">" or "›" => Keywords.IsGreaterThan,
            "decrement" or "𐑛𐑧𐑒𐑮𐑧𐑥𐑧𐑯𐑑" => Keywords.Decrement,
            "isgreaterthanorequalto" or ">=" or "›=" => Keywords.IsGreaterThanOrEqualTo,

            "increment" or "𐑦𐑯𐑒𐑮𐑧𐑥𐑧𐑯𐑑" => Keywords.Increment,

            "." => Keywords.EOS,

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
            _ => Primitives.NotBase
        }; 
        return word != Primitives.NotBase;
    }
}
