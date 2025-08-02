
using Arcane.Carmen.Lexer;

namespace Arcane.Carmen.Parser;

public partial class CarmenParser
{
    /// <summary>
    /// Remove comment and whitespace tokens.
    /// Find complex token chains and combine into single tokens.
    /// (ex. 'is' 'less' 'than' -> 'is' 'less than')
    /// </summary>
    /// <param name="tokens">Tokens to process.</param>
    /// <returns>Processed tokens.</returns>
    private List<CarmenToken> Preprocess(List<Token> tokens)
    {
        Log("Clearing comments and empty tokens from the passed tokens.");
        var processed = new List<CarmenToken>();
        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (token == null || token.Content.Length == 0 || token.Type == TokenType.Whitespace || token.Type == TokenType.Comment)
                continue; // Skip token.
            _ = token.TryMatchKeyword(out var key);
            var ct = new CarmenToken(token.Content, token.Position, token.Type, key);
            processed.Add(ct);
        }
        Log("Condensing complex tokens.");
        var pass = new List<CarmenToken>();
        for (int i = 0; i < processed.Count; i++)
        {
            var token = processed[i];
            // Condense tokens

            // ArrayStride Helper
            if (token.IsKeyword(Keywords.Element) && i + 1 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.Of))
            {
                pass.Add(new("element of", token.Position, TokenType.Operators, Keywords.ElementOf));
                i++;
                break;
            }

            // Genitive
            if (token.IsKeyword(Keywords.Apostrophe) && i + 1 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.S))
            {
                pass.Add(new("'s", token.Position, TokenType.Operators, Keywords.ApostrophyS));
                i++;
                break;
            }

            // Concatenation
            if (token.IsKeyword(Keywords.Concatenated) && i + 1 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.With))
            {
                pass.Add(new("concatenated with", token.Position, TokenType.Operators, Keywords.ConcatenatedWith));
                i++;
                continue;
            }

            // Null Coalesce
            if (token.IsKeyword(Keywords.Comma) && i + 5 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.If) &&
                processed[i + 2].IsKeyword(Keywords.Not) &&
                processed[i + 3].IsKeyword(Keywords.Null) &&
                processed[i + 4].IsKeyword(Keywords.Comma) &&
                processed[i + 5].IsKeyword(Keywords.Otherwise))
            {
                pass.Add(new(", if not null, otherwise", token.Position, TokenType.Operators, Keywords.IfNotNullOtherwise));
                i += 5;
                break;
            }
            
            // Bitwise operators
            if (token.IsKeyword(Keywords.Bitwise) && i + 1 < processed.Count)
            {
                switch (processed[i + 1].Keyword)
                {
                    case Keywords.Not:
                        pass.Add(new("bitwise not", token.Position, TokenType.Operators, Keywords.BitwiseNot));
                        i++;
                        break;
                    case Keywords.And:
                        pass.Add(new("bitwise and", token.Position, TokenType.Operators, Keywords.BitwiseAnd));
                        i++;
                        break;
                    case Keywords.Or:
                        pass.Add(new("bitwise or", token.Position, TokenType.Operators, Keywords.BitwiseOr));
                        i++;
                        break;
                    case Keywords.Xor:
                        pass.Add(new("bitwise xor", token.Position, TokenType.Operators, Keywords.BitwiseXor));
                        i++;
                        break;
                }
            }
            if (token.IsKeyword(Keywords.Shifted) && i + 2 < processed.Count &&
                processed[i + 2].IsKeyword(Keywords.By))
            {
                switch (processed[i + 1].Keyword)
                {
                    case Keywords.Left:
                        pass.Add(new("shifted left by", token.Position, TokenType.Operators, Keywords.ShiftedLeftBy));
                        i += 2;
                        break;
                    case Keywords.Right:
                        pass.Add(new("shifted right by", token.Position, TokenType.Operators, Keywords.ShiftedRightBy));
                        i += 2;
                        break;
                }
            }
            if (token.IsKeyword(Keywords.Rotated) && i + 2 < processed.Count &&
                processed[i + 2].IsKeyword(Keywords.By))
            {
                switch (processed[i + 1].Keyword)
                {
                    case Keywords.Left:
                        pass.Add(new("rotated left by", token.Position, TokenType.Operators, Keywords.RotatedLeftBy));
                        i += 2;
                        break;
                    case Keywords.Right:
                        pass.Add(new("rotated right by", token.Position, TokenType.Operators, Keywords.RotatedRightBy));
                        i += 2;
                        break;
                }
            }


            // AddressOd operator
            if (token.IsKeyword(Keywords.The) && i + 2 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.Address) &&
                processed[i + 2].IsKeyword(Keywords.Of))
            {
                pass.Add(new("the address of", token.Position, TokenType.Operators, Keywords.TheAddressOf));
                i += 2;
                continue;
            }

            // Comparison operators
            if (token.Keyword == Keywords.Is)
            {
                if (i + 2 < tokens.Count)
                {
                    if (processed[i + 1].Keyword == Keywords.Equal &&
                        processed[i + 2].Keyword == Keywords.To)
                    {
                        pass.Add(new("is equal to", token.Position, TokenType.Operators, Keywords.IsEqualTo));
                        i += 2;
                        continue;
                    }
                    else if (processed[i + 1].Keyword == Keywords.Less &&
                        processed[i + 2].Keyword == Keywords.Than)
                    {
                        if (i + 5 < processed.Count &&
                            processed[i + 3].Keyword == Keywords.Or &&
                            processed[i + 4].Keyword == Keywords.Equal &&
                            processed[i + 5].Keyword == Keywords.To)
                        {
                            pass.Add(new("is less than or equal to", token.Position, TokenType.Operators, Keywords.IsLessThanOrEqualTo));
                            i += 5;
                            continue;
                        }
                        pass.Add(new("is less than", token.Position, TokenType.Operators, Keywords.IsLessThan));
                        i += 2;
                        continue;
                    }
                    else if (processed[i + 1].Keyword == Keywords.Greater &&
                        processed[i + 2].Keyword == Keywords.Than)
                    {
                        if (i + 5 < processed.Count &&
                            processed[i + 3].Keyword == Keywords.Or &&
                            processed[i + 4].Keyword == Keywords.Equal &&
                            processed[i + 5].Keyword == Keywords.To)
                        {
                            pass.Add(new("is greater than or equal to", token.Position, TokenType.Operators, Keywords.IsGreaterThanOrEqualTo));
                            i += 5;
                            continue;
                        }
                        pass.Add(new("is greater than", token.Position, TokenType.Operators, Keywords.IsGreaterThan));
                        i += 2;
                        continue;
                    }
                }
                else if (i + 3 < tokens.Count)
                {
                    if (processed[i + 1].Keyword == Keywords.Not)
                    {
                        if (processed[i + 2].Keyword == Keywords.Equal &&
                            processed[i + 3].Keyword == Keywords.To)
                        {
                            pass.Add(new("is not equal to", token.Position, TokenType.Operators, Keywords.IsNotEqualTo));
                            i += 3;
                            continue;
                        }
                        else if (processed[i + 2].Keyword == Keywords.Less &&
                            processed[i + 3].Keyword == Keywords.Than)
                        {
                            if (i + 6 < processed.Count &&
                                processed[i + 4].Keyword == Keywords.Or &&
                                processed[i + 5].Keyword == Keywords.Equal &&
                                processed[i + 6].Keyword == Keywords.To)
                            {
                                pass.Add(new("is not less than or equal to", token.Position, TokenType.Operators, Keywords.IsGreaterThan));
                                i += 5;
                                continue;
                            }
                            pass.Add(new("is not less than", token.Position, TokenType.Operators, Keywords.IsGreaterThanOrEqualTo));
                            i += 3;
                            continue;
                        }
                        else if (processed[i + 2].Keyword == Keywords.Greater &&
                            processed[i + 3].Keyword == Keywords.Than)
                        {
                            if (i + 6 < processed.Count &&
                                processed[i + 4].Keyword == Keywords.Or &&
                                processed[i + 5].Keyword == Keywords.Equal &&
                                processed[i + 6].Keyword == Keywords.To)
                            {
                                pass.Add(new("is not greater than or equal to", token.Position, TokenType.Operators, Keywords.IsLessThan));
                                i += 5;
                                continue;
                            }
                            pass.Add(new("is not greater than", token.Position, TokenType.Operators, Keywords.IsLessThanOrEqualTo));
                            i += 3;
                            continue;
                        }
                    }
                }
            }
            // Var Definition operator
            if (token.IsKeyword(Keywords.Is)) {
                if (i + 1 < processed.Count &&
                    processed[i + 1].IsKeyword(Keywords.A))
                {
                    pass.Add(new("is a", token.Position, TokenType.Operators, Keywords.IsA));
                    i++;
                    continue;
                }
                if (i + 2 < processed.Count &&
                    processed[i + 1].IsKeyword(Keywords.Not) &&
                    processed[i + 2].IsKeyword(Keywords.A))
                {
                    pass.Add(new("is not a", token.Position, TokenType.Operators, Keywords.IsNotA));
                    i += 2;
                    continue;
                }
            }
            // Cast Type operator
            if (token.IsKeyword(Keywords.As) && i + 1 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.A))
            {
                pass.Add(new("as a", token.Position, TokenType.Operators, Keywords.AsA));
                i++;
                continue;
            }


            // Iterator compounds
            if (token.IsKeyword(Keywords.As) && i + 1 < processed.Count)
            {
                if (processed[i + 1].IsKeyword(Keywords.Index))
                {
                    pass.Add(new("as index", token.Position, TokenType.Operators, Keywords.AsIndex));
                    i++;
                    continue;
                }
                if (processed[i + 1].IsKeyword(Keywords.Value))
                {
                    pass.Add(new("as value", token.Position, TokenType.Operators, Keywords.AsValue));
                    i++;
                    continue;
                }
            }
            if (token.IsKeyword(Keywords.For) && i + 1 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.Each))
            {
                pass.Add(new("for each", token.Position, TokenType.Operators, Keywords.ForEach));
                i++;
                continue;
            }
            if (token.IsKeyword(Keywords.Iterate) && i + 1 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.Over))
            {
                pass.Add(new("as a", token.Position, TokenType.Operators, Keywords.IterateOver));
                i++;
                continue;
            }


            // Array size operator
            if (token.Keyword == Keywords.With && i + 1 < processed.Count &&
                processed[i + 1].Keyword == Keywords.Size)
            {
                pass.Add(new("with size", token.Position, TokenType.Operators, Keywords.WithSize));
                i++;
                continue;
            }
            // Array type operator
            if (token.Keyword == Keywords.Of && i + 1 < processed.Count && 
                processed[i + 1].Keyword == Keywords.Type)
            {
                pass.Add(new("of type", token.Position, TokenType.Operators, Keywords.OfType));
                i++;
                continue;
            }

            // Pointer Definition
            if (token.Keyword == Keywords.Pointer && i + 1 < processed.Count &&
                processed[i + 1].Keyword == Keywords.To)
            {
                pass.Add(new("pointer to", token.Position, TokenType.Operators, Keywords.PointerTo));
                i++;
                continue;
            }

            // Assignment operator
            if (token.Keyword == Keywords.Equal &&
                i + 1 < processed.Count &&
                processed[i + 1].Keyword == Keywords.To)
            {
                pass.Add(new("equal to", token.Position, TokenType.Operators, Keywords.EqualTo));
                i++;
                continue;
            }

            // Math Operations
            if (token.IsKeyword(Keywords.The) && i + 2 < processed.Count &&
                processed[i + 2].IsKeyword(Keywords.Of))
            {
                switch (processed[i + 1].Keyword)
                {
                    case Keywords.Sum:
                        pass.Add(new("the sum of", token.Position, TokenType.Operators, Keywords.TheSumOf));
                        i += 2;
                        continue;
                    case Keywords.Difference:
                        pass.Add(new("the difference of", token.Position, TokenType.Operators, Keywords.TheDifferenceOf));
                        i += 2;
                        continue;
                    case Keywords.Product:
                        pass.Add(new("the product of", token.Position, TokenType.Operators, Keywords.TheProductOf));
                        i += 2;
                        continue;
                    case Keywords.Quotient:
                        pass.Add(new("the quotient of", token.Position, TokenType.Operators, Keywords.TheQuotientOf));
                        i += 2;
                        continue;
                    case Keywords.Modulo:
                        pass.Add(new("the modulo of", token.Position, TokenType.Operators, Keywords.TheModuloOf));
                        i += 2;
                        continue;
                }
            }
            if (token.IsKeyword(Keywords.Root) && i + 1 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.Of))
            {
                pass.Add(new("root of", token.Position, TokenType.Operators, Keywords.RootOf));
                i++;
                continue;
            }
            if (token.IsKeyword(Keywords.Raised) && i + 4 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.To) &&
                processed[i + 2].IsKeyword(Keywords.The) &&
                processed[i + 3].IsKeyword(Keywords.Power) &&
                processed[i + 4].IsKeyword(Keywords.Of))
            {
                pass.Add(new("raised to the power of", token.Position, TokenType.Operators, Keywords.RaisedToThePowerOf));
                i += 4;
                continue;
            }
            if (token.IsKeyword(Keywords.Divided) && i + 1 < processed.Count &&
                processed[i + 1].IsKeyword(Keywords.By))
            {
                pass.Add(new("divided by", token.Position, TokenType.Operators, Keywords.Divider));
            }

            pass.Add(token);
        }
        return pass;
    }
}
