using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Expressions
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

    public static class IdentifierTypeEx
    {
        /// <summary>
        /// Check if identifies a type of storable value in case i want to add classes/records and when i add enum definitions
        /// !!! Will return true on alias further queries will be required for checking. !!!
        /// </summary>
        /// <param name="type">Type of identifier.</param>
        /// <returns>Is it a type of storable value?.</returns>
        public static bool IsValueType(this IdentifierType type)
        {
            return type switch
            {
                IdentifierType.Alias or
                IdentifierType.Type or
                IdentifierType.Structure => true,
                _ => false
            };
        }
        /// <summary>
        /// Is a type that has members
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsContainerType(this IdentifierType type) 
        {
            return type switch
            {
                IdentifierType.Structure => true,
                _ => false
            };
        }
    }

    public record ExprIdentifier(string Name, IdentifierType Type) : Expression;

    public class ExprIdentifierParser : ExpressionParser
    {
        public ExprIdentifierParser()
            : base(ExpressionPriorities.Identifier) { }
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
            expression = new ExprIdentifier(
                token.Type switch
                {
                    TokenType.TypeString => "string",
                    TokenType.TypeBoolean => "bool",
                    TokenType.TypeByte => "byte",
                    TokenType.TypeChar => "char",
                    TokenType.TypeShort => "short",
                    TokenType.TypeInt => "int",
                    TokenType.TypeLong => "long",
                    TokenType.TypeFloat => "float",
                    TokenType.TypeDouble => "double",
                    TokenType.TypeDecimal => "decimal",
                    TokenType.TypeObject => "object",
                    TokenType.TypeStruct => "struct",
                    _ => tokens[0].Raw
                },
                type);
            return true;
        }
    }
}
