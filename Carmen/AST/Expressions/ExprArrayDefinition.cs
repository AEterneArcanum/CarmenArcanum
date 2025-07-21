using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprArrayDefinition(ExprIdentifier Type, Expression? Size) : Expression;

    public class ExprArrayDefinitionParser : ExpressionParser
    {
        public ExprArrayDefinitionParser(int priority) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? result) // 'array' ('of size' SIZE)? 'of' TYPE
        {
            result = null;
            if (tokens.Length < 3 || tokens[0].Type != TokenType.KeywordArray)
            {
                return false; // Not enough tokens or does not start with 'array'
            }
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordOf, out var idxOf)) return false;
            // Check for 'of size' keyword
            bool definedSize = tokens[1].Type == TokenType.KeywordOfSize;
            Expression? size = null;
            if (definedSize)
            {
                if (idxOf < 2 || idxOf >= tokens.Length - 1) return false; // 'of size' must be followed by a size and then 'of'
                if (!Expression.TryParse(tokens[2..idxOf], out size)) return false; // Parse the size expression
            }
            else
            {
                if (idxOf < 1 || idxOf >= tokens.Length - 1) return false; // 'of' must be followed by a type
            }

            if (!Expression.TryParse(tokens[(idxOf + 1)..], out Expression? type) ||
                type is not ExprIdentifier || !((ExprIdentifier)type).IsType())
            {
                return false; // Failed to parse the type expression
            }
            // Create the array definition expression
            result = new ExprArrayDefinition((ExprIdentifier)type!, size);
            return true; // Successfully parsed the array definition expression
        }
    }
}
