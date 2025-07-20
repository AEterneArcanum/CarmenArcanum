using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public enum IdentifierType
    {
        Variable,
        Function,
        Structure,
        Label,
        Alias,
        Type
    }

    public record ExprIdentifier(string Name, IdentifierType Type) : Expression
    {
        public override string ToString() => Name;
    }

    public class ExprIdentifierParser : ExpressionParser
    {
        public ExprIdentifierParser(int priority = ExpressionPriorities.Identifier) : base(priority) // Assuming lowest priority for identifiers
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length != 1) return false;
            var token = tokens[0];
            if (!token.Type.IsIdentifier() && token.Type != TokenType.Unknown) // Unknown may be used for dynamic identifiers
                return false;
            var type = token.Type switch
            {
                TokenType.VariableIdentifier => IdentifierType.Variable,
                TokenType.FuncIdentifier => IdentifierType.Function,
                TokenType.StructIdentifier => IdentifierType.Structure,
                TokenType.LabelIdentifier => IdentifierType.Label,
                TokenType.AliasIdentifier => IdentifierType.Alias,
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
                TokenType.TypeStruct => IdentifierType.Type,
                _ => throw new ArgumentException("Invalid token type for identifier parsing.")
            };
            expression = new ExprIdentifier(token.Raw, type);
            return true;
        }
    }
}
