using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Lexer.Tokens
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
            return FirstTopLayerIndexOfFrom(tokens, type, 0);
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

        public static bool IsDecimalPoint(this Token token)
        {
            return token.Raw == Symbols.Point;
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

        public static bool TryConvertToDecimal(this Token token, out decimal result)
        {
            if (token.IsNumeric())
            {
                result = 0;
                return false;
            }
            if (decimal.TryParse(token.Raw, out var decimalValue))
            {
                result = (long)decimalValue; // Convert decimal to long.
                return true;
            }
            result = token.Raw switch
            {
                //"0" => 0L, // Explicitly handle "0" as a long. "0" does not parse automatically.

                Symbols.Zero => 0,
                Symbols.One => 1,
                Symbols.Two => 2,
                Symbols.Three => 3,
                Symbols.Four => 4,
                Symbols.Five => 5,
                Symbols.Six => 6,
                Symbols.Seven => 7,
                Symbols.Eight => 8,
                Symbols.Nine => 9,
                Symbols.Ten => 10,
                Symbols.Eleven => 11,
                Symbols.Twelve => 12,
                Symbols.Thirteen => 13,
                Symbols.Fourteen => 14,
                Symbols.Fifteen => 15,
                Symbols.Sixteen => 16,
                Symbols.Seventeen => 17,
                Symbols.Eighteen => 18,
                Symbols.Nineteen => 19,
                Symbols.Twenty => 20,
                Symbols.Thirty => 30,
                Symbols.Forty => 40,
                Symbols.Fifty => 50,
                Symbols.Sixty => 60,
                Symbols.Seventy => 70,
                Symbols.Eighty => 80,
                Symbols.Ninety => 90,
                Symbols.Hundred => 100,
                Symbols.Thousand => 1000,
                Symbols.Million => 1000000,
                Symbols.Billion => 1000000000,
                Symbols.Trillion => 1000000000000,
                Symbols.Quadrillion => 1000000000000000,
                Symbols.Quintillion => 1000000000000000000,

                Symbols.First => 1,
                Symbols.Second => 2,
                Symbols.Third => 3,
                Symbols.Fourth => 4,
                Symbols.Fifth => 5,
                Symbols.Sixth => 6,
                Symbols.Seventh => 7,
                Symbols.Eighth => 8,
                Symbols.Ninth => 9,
                Symbols.Tenth => 10,
                Symbols.Eleventh => 11,
                Symbols.Twelfth => 12,
                Symbols.Thirteenth => 13,
                Symbols.Fourteenth => 14,
                Symbols.Fifteenth => 15,
                Symbols.Sixteenth => 16,
                Symbols.Seventeenth => 17,
                Symbols.Eighteenth => 18,
                Symbols.Nineteenth => 19,
                Symbols.Twentieth => 20,
                Symbols.Thirtieth => 30,
                Symbols.Fortieth => 40,
                Symbols.Fiftieth => 50,
                Symbols.Sixtieth => 60,
                Symbols.Seventieth => 70,
                Symbols.Eightieth => 80,
                Symbols.Ninetieth => 90,
                Symbols.Hundredth => 100,
                Symbols.Thousandth => 1000,
                Symbols.Millionth => 1000000,
                Symbols.Billionth => 1000000000,
                Symbols.Trillionth => 1000000000000,
                Symbols.Quadrillionth => 1000000000000000,
                Symbols.Quintillionth => 1000000000000000000,
                Symbols.Point => 0.0m, // Decimal point as a fraction.
                _ => throw new Exception($"Unknown numeric token: {token.Raw} of type {token.Type} at line {token.Line}, column {token.Column}")
            };
            return true;
        }

        public static bool TryConvertToDecimal(this Token[] tokens, out decimal result)
        {
            if (!tokens.IsNumeric())
            {
                result = 0;
                return false; // Not all tokens are numeric.
            }

            result = 0;
            if (tokens.Length == 1)
            {
                return tokens[0].TryConvertToDecimal(out result);
            }
            else
            {
                bool inDecimal = false;
                decimal multiplier = 1.0m;

                for (var i = 0; i < tokens.Length; i++) 
                {
                    Token token = tokens[i];
                    if (!token.TryConvertToDecimal(out var value))
                    {
                        result = 0;
                        return false; // Non-numeric token
                    }
                    if (token.IsDecimalPoint())
                    {
                        if (inDecimal)
                        {
                            result = 0;
                            return false; // Multiple decimal points
                        }
                        inDecimal = true; // Enter decimal mode.
                        multiplier = 1; // Reset multiplier for decimal.
                        continue; // Skip the point, we handle it in the next iteration.
                    }
                    if (inDecimal)
                    {
                        // If we are in decimal mode, we need to adjust the multiplier.
                        multiplier *= 0.1m;
                    }
                    
                    switch (token.Raw)
                    {
                        case Symbols.Hundred:
                        case Symbols.Hundredth:
                        case Symbols.Thousand:
                        case Symbols.Thousandth:
                        case Symbols.Million:
                        case Symbols.Millionth:
                        case Symbols.Billion:
                        case Symbols.Billionth:
                        case Symbols.Trillion:
                        case Symbols.Trillionth:
                        case Symbols.Quadrillion:
                        case Symbols.Quadrillionth:
                        case Symbols.Quintillion:
                        case Symbols.Quintillionth: // Positional
                            if (result == 0)
                            {
                                result = 1; // If we are at the start, treat it as 1.
                            }
                            result *= value; // Multiply the current result by 1,000,000,000,000,000,000.
                            break;
                        default:
                            result += value * multiplier; // For all other numeric values, just add them to the result.
                            break; // Break after processing the token.
                    }
                }

                return true; // Successfully converted to decimal.
            }
        }
    }
}
