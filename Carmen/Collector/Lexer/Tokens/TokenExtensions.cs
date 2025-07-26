using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.Lexer.Tokens
{
    public static class TokenExtensions
    {
        /// <summary>
        /// Will get the first index of specified type typically keywords outside of parenthesised tokens.
        /// </summary>
        /// <param name="tokens">Tokens to search</param>
        /// <param name="type">Type to find</param>
        /// <returns></returns>
        public static int FirstTopLayerIndexOf(this Token[] tokens, TokenType type)
        {
            return tokens.FirstTopLayerIndexOfFrom(type, 0);
        }
        public static int FirstTopLayerIndexOfFrom(this Token[] tokens, TokenType type, int startIndex)
        {
            int layer = 0; // +1 on Open Paren, -1 on Close : Only a successful find on layer zero really an error should be thrown if negative.
            for (int i = startIndex; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (token.Type == TokenType.PunctuationOpenParenthesis || token.Type == TokenType.BlockStart)
                {
                    layer++;
                }
                else if (token.Type == TokenType.PunctuationCloseParenthesis || token.Type == TokenType.BlockEnd)
                {
                    layer--;
                }
                else if (layer == 0 && token.Type == type)
                {
                    return i; // Return the index of the first token of the specified type at top level.
                }
            }
            return -1; // If we reach here, it means we didn't find the token at top level, return the layer count.
        }
        public static bool TryGetFirstTopLayerIndexOfFrom(this Token[] tokens, TokenType[] types, int start, out int index)
        {
            index = -1;
            if (tokens is null || types is null || start < 0 || start >= tokens.Length) return false;
            int depth = 0;
            for (int i = start + 1; i < tokens.Length; i++)
            {
                var token = tokens[i].Type;
                if (token == TokenType.BlockStart) { depth++; }
                else if (token == TokenType.BlockEnd) { 
                    depth--;
                    if (depth == 0 && types.Contains(TokenType.BlockEnd))
                    {
                        index = i; break;
                    }
                    if (depth < 0) break;
                }
                else if (depth == 0 && types.Contains(token)) { index = i; break; }
            }
            return index != -1;
        }
        public static bool TryGetFirstTopLayerIndexOfFrom(this Token[] tokens, TokenType type, int start, out int index)
        {
            if (!tokens.Any(t => t.Type == type))
            {
                index = -1; // If the type is not found, return -1.
                return false;
            }
            index = tokens.FirstTopLayerIndexOfFrom(type, start);
            return index >= 0; // If index is -1, it means the token was not found at top level.
        }
        public static bool TryGetFirstTopLayerIndexOf(this Token[] tokens, TokenType type, out int index)
        {
            if (!tokens.Any(t => t.Type == type))
            {
                index = -1; // If the type is not found, return -1.
                return false;
            }
            index = tokens.FirstTopLayerIndexOf(type);
            return index >= 0; // If index is -1, it means the token was not found at top level.
        }
        public static List<int> TopLayerIndicesOf(this Token[] tokens, TokenType type)
        {
            List<int> indices = new List<int>();
            int layer = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (token.Type == TokenType.PunctuationOpenParenthesis || token.Type == TokenType.BlockStart)
                {
                    layer++;
                }
                else if (token.Type == TokenType.PunctuationCloseParenthesis || token.Type == TokenType.BlockEnd)
                {
                    layer--;
                }
                else if (layer == 0 && token.Type == type)
                {
                    indices.Add(i);
                }
            }
            return indices;
        }
        public static List<int> TopLayerIndicesOfXBeforeY(this Token[] tokens, TokenType x, TokenType y)
        {
            List<int> indices = new List<int>();
            int layer = 0; // +1 on Open Paren, -1 on Close : Only a successful find on layer zero really an error should be thrown if negative.

            for (int i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                if (token.Type == TokenType.PunctuationOpenParenthesis || token.Type == TokenType.BlockStart)
                {
                    layer++;
                }
                else if (token.Type == TokenType.PunctuationCloseParenthesis || token.Type == TokenType.BlockEnd)
                {
                    layer--;
                }
                else if (layer == 0 && token.Type == x)
                {
                    indices.Add(i);
                }
                else if (layer == 0 && token.Type == y)
                {
                    break; // Stop searching when we hit the first y at top level.
                }
            }
            return indices;
        }

        public static bool IsNumeric(this Token token)
        {
            return token.Type == TokenType.LiteralNumber || token.Type == TokenType.LiteralPosition;
        }

        public static bool IsNumeric(this Token[] tokens)
        {
            if (tokens.Length == 0) return false;
            // Check if all tokens are numeric.
            return tokens.All(t => t.IsNumeric());
        }
    }
}
