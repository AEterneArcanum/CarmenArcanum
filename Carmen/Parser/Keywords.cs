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
            "isgreaterthanorequalto" or ">=" or "›=" => Keywords.IsGreaterThanOrEqualTo,

            "decrement" or "𐑛𐑧𐑒𐑮𐑧𐑥𐑧𐑯𐑑" => Keywords.Decrement,
            "increment" or "𐑦𐑯𐑒𐑮𐑧𐑥𐑧𐑯𐑑" => Keywords.Increment,

            "." => Keywords.EOS,

            _ => Keywords.Unknown
        };
        return word != Keywords.Unknown;
    }

    public static bool TryMatchBaseType(this Token typeword, out BasicTypes word)
    {
        word = typeword.Content switch
        {
            "byte" or "𐑚𐑲𐑑" or
            "bytes" or "𐑚𐑲𐑑𐑕" => BasicTypes.Byte,
            "short" or "𐑖𐑹𐑑" or
            "shorts" or "𐑖𐑹𐑑𐑕" => BasicTypes.Short,
            _ => BasicTypes.NotBase
        }; 
        return word != BasicTypes.NotBase;
    }
}
