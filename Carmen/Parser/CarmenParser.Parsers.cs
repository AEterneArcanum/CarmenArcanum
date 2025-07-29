using Arcane.Carmen.AST;
using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Statements;
using Arcane.Carmen.AST.Types;
using Arcane.Carmen.Lexer;
using System.Diagnostics.CodeAnalysis;

namespace Arcane.Carmen.Parser;

public partial class CarmenParser
{
    private bool TryParse(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        try
        {
            Debug($"Attempting to parse \"{tokens.AsString()}\" as a statement...");
            if (TryParseBlock(tokens, out output)
                || TryParseIf(tokens, out output)
                || TryParseAssignment(tokens, out output)
                || TryParseVarDef(tokens, out output)
                || TryParseGoto(tokens, out output)
                || TryParseLabel(tokens, out output)
                || TryParseEPoint(tokens, out output)
                || TryParseDecrement(tokens, out output)
                || TryParseIncrement(tokens, out output))
            {
                return true;
            }
            // Removed expressions save for inc/dec are not allowed as statements
            //ASTExpression? parsedExpr = null;
            //if (TryParseExpression(tokens, out parsedExpr))
            //{
            //    output = parsedExpr;
            //    return true;
            //}
            return false;
        }
        catch (ParserException ex)
        {
            Error(ex.Function, ex.Message, [.. ex.Tokens]);
        }
        output = null;
        return false;
    }

    private bool TryParseExpression(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        Debug($"Attempting to parse \"{tokens.AsString()}\" as an expression...");
        if (TryParseParenthized(tokens, out output)
            || TryParseLitBool(tokens, out output)
            || TryParseLitNumber(tokens, out output)
            || TryParseLitString(tokens, out output)
            || TryParseLitChar(tokens, out output)
            || TryParseLitNull(tokens, out output)
            || TryParseIdentifier(tokens, out output)
            || TryParseMemberAccess(tokens, out output)
            || TryParseComparison(tokens, out output)
            || TryParseArrayAccess(tokens, out output)
            || TryParseAddressOf(tokens, out output)
            || TryParseLitList(tokens, out output)
            || TryParseMathOp(tokens, out output)
            || TryParseConcat(tokens, out output)
            || TryParseNullCoalesce(tokens, out output)
            || TryParseBooleanOp(tokens, out output)
            || TryParseBitwiseOp(tokens, out output)
            || TryParseBooleanNot(tokens, out output)
            || TryParseBitwiseNot(tokens, out output))
        {
            return true;
        }
        else if (TryParseDecrement(tokens, out var outnd)
            || TryParseIncrement(tokens, out outnd))
        {
            output = (ASTExpression)outnd;
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool TryParseMemberAccess(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length < 3) return false;
        int ofGen = tokens.NextTopLevelIndexOf(Keywords.ApostrophyS);
        if (ofGen < 0) return false;
        if (ofGen < 1 || ofGen >= tokens.Length - 1) return false;
        if (!TryParseExpression(tokens, out var possessor)) return false;
        if (!TryParseExpression(tokens, out var member)) return false;
        output = new ASTMemberAccess(tokens.GetASTPosition(), possessor, member);
        return true;
    }

    private bool TryParseConcat(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null; 
        if (tokens.Length < 3) return false;
        int idxOp = tokens.NextTopLevelIndexOf(Keywords.ConcatenatedWith);
        if (idxOp < 1 || idxOp >= tokens.Length - 1) return false;
        if (!TryParseExpression(tokens[..idxOp], out var left)) return false;
        if (!TryParseExpression(tokens[(idxOp + 1)..], out var right)) return false;
        output = new ASTConcat(tokens.GetASTPosition(), left, right);
        return true;
    }

    private bool TryParseNullCoalesce(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length < 3) return false;
        int idxOp = tokens.NextTopLevelIndexOf(Keywords.IfNotNullOtherwise);
        if (idxOp < 1 || idxOp >= tokens.Length - 1) return false;
        if (!TryParseExpression(tokens[..idxOp], out var ValueNode)) return false;
        if (!TryParseExpression(tokens[(idxOp + 1)..], out var AltNode)) return false;
        output = new ASTNullCoalesce(tokens.GetASTPosition(), ValueNode, AltNode);
        return true;
    }

    private bool TryParseBitwiseOp(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length < 3) return false;
        Keywords[] opwords = [
            Keywords.BitwiseOr,
            Keywords.BitwiseAnd,
            Keywords.BitwiseXor,
            Keywords.ShiftedLeftBy,
            Keywords.ShiftedRightBy,
            Keywords.RotatedLeftBy,
            Keywords.RotatedRightBy,
            ];
        int idxOp = tokens.NextTopLevelIndexOf(opwords, out var match);
        if (idxOp == -1) return false;
        ASTBitwiseOp code = match switch
        {
            Keywords.BitwiseOr => ASTBitwiseOp.OR,
            Keywords.BitwiseAnd => ASTBitwiseOp.AND,
            Keywords.BitwiseXor => ASTBitwiseOp.XOR,
            Keywords.ShiftedLeftBy => ASTBitwiseOp.ShiftLeft,
            Keywords.ShiftedRightBy => ASTBitwiseOp.ShiftRight,
            Keywords.RotatedLeftBy => ASTBitwiseOp.RotateLeft,
            Keywords.RotatedRightBy => ASTBitwiseOp.RotateRight,
            _ => throw new InvalidProgramException($"Invalid code reached in {nameof(TryParseBitwiseOp)}")
        };
        if (!TryParseExpression(tokens, out var left))
        {
            Warning(nameof(TryParseBitwiseOp),
                "Attempted to parse bitwise expression _ left operand parse failed.",
                [.. tokens]);
            return false;
        }
        if (!TryParseExpression(tokens, out var right))
        {
            Warning(nameof(TryParseBitwiseOp),
                "Attempted to parse bitwise expression _ right operand parse failed.",
                [.. tokens]);
            return false;
        }
        output = new ASTBitwise(tokens.GetASTPosition(), code, left, right);
        return true;
    }

    private bool TryParseBitwiseNot(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        // General fast fail.
        if (tokens.Length < 2 || tokens[0].IsNotKeyword(Keywords.BitwiseNot))
            return false;
        if (!TryParseExpression(tokens, out var innerExpr))
            Throw(nameof(TryParseBitwiseNot), "Failed to parse unitary inner expression.", tokens[1..]);
        output = new ASTBitwiseNot(tokens.GetASTPosition(), innerExpr!);
        return true;
    }

    private bool TryParseMathOp(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length < 2) return false;
        // Check for trailing squared or cubed
        if (tokens[^1].IsKeyword(Keywords.Squared))
        {
            if (!TryParseExpression(tokens, out var innerExpr))
                return false;
            output = new ASTMath(tokens.GetASTPosition(),
                ASTMathOp.Power, innerExpr!, new ASTLitNumber(tokens.GetASTPosition(), 2));
        }
        else if (tokens[^1].IsKeyword(Keywords.Cubed))
        {
            if (!TryParseExpression(tokens, out var innerExpr))
                return false;
            output = new ASTMath(tokens.GetASTPosition(),
                ASTMathOp.Power, innerExpr!, new ASTLitNumber(tokens.GetASTPosition(), 3));
        }

        if (tokens.Length < 3) return false; // General Fast Fail
        Keywords[] operators = [
            // Powers
            Keywords.RaisedToThePowerOf,
            Keywords.RootOf,
            // Multipliers
            Keywords.Multiplier,
            Keywords.TheProductOf,
            // Divisions
            Keywords.Divider,
            Keywords.TheQuotientOf,
            // Addition
            Keywords.Adder,
            Keywords.TheSumOf,
            //Subtraction
            Keywords.Subtractor,
            Keywords.TheDifferenceOf,
            // Special
            Keywords.Modulo,
            Keywords.TheModuloOf,
            ];
        int idxOperator = tokens.NextTopLevelIndexOf(operators, out var match);
        if (idxOperator == -1) return false; // No Operator Clean Fail
        ASTMathOp opCode = match switch
        {
            Keywords.RaisedToThePowerOf => ASTMathOp.Power,
            Keywords.RootOf => ASTMathOp.Root,
            Keywords.Multiplier or
            Keywords.TheProductOf => ASTMathOp.Multiply,
            Keywords.Divider or
            Keywords.TheQuotientOf => ASTMathOp.Divide,
            Keywords.Adder or
            Keywords.TheSumOf => ASTMathOp.Add,
            Keywords.Subtractor or
            Keywords.TheDifferenceOf => ASTMathOp.Subtract,
            Keywords.Modulo or
            Keywords.TheModuloOf => ASTMathOp.Modulo,
            _ => throw new InvalidProgramException("Invalid code reached!")
        };
        switch (match)
        {
            case Keywords.RootOf:
                // Check for leading square or cubic
                if (idxOperator < 1 || idxOperator >= tokens.Length - 1) return false; // Warn Here
                ASTExpression? left;
                if (idxOperator == 1 && tokens[0].IsKeyword(Keywords.Square))
                    left = new ASTLitNumber(tokens.GetASTPosition(), 2);
                else if (idxOperator == 1 && tokens[0].IsKeyword(Keywords.Cubic))
                    left = new ASTLitNumber(tokens.GetASTPosition(), 3);
                else if (TryParseExpression(tokens[..idxOperator], out var lop3))
                {
                    left = lop3;
                }
                else return false;
                if (!TryParseExpression(tokens[(idxOperator + 1)..], out var rop3)) 
                { return false; }
                output = new ASTMath(tokens.GetASTPosition(), opCode, left, rop3);
                return true;
            case Keywords.RaisedToThePowerOf:
            case Keywords.Multiplier:
            case Keywords.Divider:
            case Keywords.Adder:
            case Keywords.Subtractor:
            case Keywords.Modulo:
                if (idxOperator < 1 || idxOperator >= tokens.Length - 1) return false; // Warn Here
                if (!TryParseExpression(tokens[..idxOperator], out var lop))
                    return false; // Warn Here
                if (!TryParseExpression(tokens[(idxOperator + 1)..], out var rop))
                    return false; // Warn Here
                output = new ASTMath(tokens.GetASTPosition(), opCode, lop, rop);
                return true;
            case Keywords.TheProductOf:
            case Keywords.TheQuotientOf:
            case Keywords.TheSumOf:
            case Keywords.TheDifferenceOf:
            case Keywords.TheModuloOf:
                if (tokens.Length < 4 || idxOperator != 0) return false; // Warn Here
                int idxAnd = tokens.NextTopLevelIndexOf(Keywords.And, 1);
                if (idxAnd == -1) return false; // Warn Here
                if (!TryParseExpression(tokens[1..idxAnd], out var lop2))
                    return false; // Warn Here
                if (!TryParseExpression(tokens[(idxAnd + 1)..], out var rop2))
                    return false; // Warn Here
                output = new ASTMath(tokens.GetASTPosition(), opCode, lop2, rop2);
                return true;
            default: throw new InvalidProgramException("Invalid Code Reached!");
        }
    }

    private bool TryParseBooleanOp(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length < 3) return false;
        Keywords[] opwords = [
            Keywords.Or,
            Keywords.And, 
            Keywords.Xor,
            Keywords.IsA,
            Keywords.IsNotA,
            ];
        int idxOperator = tokens.NextTopLevelIndexOf(opwords, out var match);
        if (idxOperator == -1) return false;
        ASTBooleanOp code = match switch
        {
            Keywords.Or => ASTBooleanOp.Or,
            Keywords.And => ASTBooleanOp.And,
            Keywords.Xor => ASTBooleanOp.Xor,
            Keywords.IsA => ASTBooleanOp.IsType,
            Keywords.IsNotA => ASTBooleanOp.IsNotType,
            _ => throw new InvalidProgramException($"Invalid code reached in {nameof(TryParseComparison)}")
        };
        if (!TryParseExpression(tokens[..idxOperator], out var leftOp)) 
        {
            Warning(nameof(TryParseBooleanOp),
                "Attempted to parse boolean expression _ left operand parse failed.",
                [.. tokens]);
            return false;
        }
        if (!TryParseExpression(tokens[(idxOperator + 1)..], out var rightOp))
        {
            Warning(nameof(TryParseBooleanOp),
                "Attempted to parse boolean expression _ right operand parse failed.",
                [.. tokens]);
            return false;
        }
        output = new ASTBoolean(tokens.GetASTPosition(), code, leftOp, rightOp);
        return true;
    }

    private bool TryParseBooleanNot(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length < 2 || tokens[0].IsNotKeyword(Keywords.Not))
            return false;
        if (!TryParseExpression(tokens[1..], out var inner))
        {
            Warning(nameof(TryParseBooleanNot), "Warning: failed to parse boolean not expression, passing on.", tokens);
            return false;
        }
        output = new ASTBooleanNot(tokens.GetASTPosition(), inner);
        return true;
    }

    private bool TryParseArrayAccess(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length < 4 || tokens[0].IsNotKeyword(Keywords.Index)) return false;
        int idx_of = tokens.NextTopLevelIndexOf(Keywords.Of);
        if (idx_of < 2) return false;
        if (!TryParseExpression(tokens[1..idx_of], out var idexpr))
            Throw(nameof(TryParseArrayAccess), "Error parsing array access index.", tokens[1..idx_of]);
        if (!TryParseExpression(tokens[(idx_of + 1)..], out var obexpr))
            Throw(nameof(TryParseArrayAccess), "Error parsing array access object.", tokens[(idx_of + 1)..]);
        output = new ASTArrayAccess(tokens.GetASTPosition(), idexpr!, obexpr!);
        return true;
    }

    private bool TryParseLitList(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length == 0)
            return false;
        if (tokens[0].IsNotKeyword(Keywords.Semicolon))
            return false;
        var parts = new List<ASTExpression>();
        var pieces = tokens[1..].Split(Keywords.Comma);
        foreach ( var part in pieces )
        {
            if (!TryParseExpression(part, out var expr))
                Throw(nameof(TryParseLitList), "Error parsing list element.", part);
            parts.Add(expr!);
        }
        output = new ASTLitList(tokens.GetASTPosition(), [..parts]);
        return true;
    }

    private bool TryParseAddressOf(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length == 0 || tokens[0].IsNotKeyword(Keywords.TheAddressOf)) 
            return false;
        if (!TryParseExpression(tokens, out var expr))
            Throw(nameof(TryParseAddressOf), "Error parsing address of expression.", tokens);
        output = new ASTAddressOf(tokens.GetASTPosition(), expr!);
        return true;
    }

    private bool TryParseComparison(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length < 3) return false; // true == true 3 min.
                                             // Look for op
        Keywords[] opwords = [
            Keywords.IsEqualTo,
        Keywords.IsNotEqualTo,
        Keywords.IsGreaterThan,
        Keywords.IsLessThan,
        Keywords.IsLessThanOrEqualTo,
        Keywords.IsGreaterThanOrEqualTo ];
        int idxOp = tokens.NextTopLevelIndexOf(opwords, out var match);
        if (idxOp == -1) return false; // no opcode
        ASTComparisonOp opCode = match switch
        {
            Keywords.IsEqualTo => ASTComparisonOp.Equal,
            Keywords.IsNotEqualTo => ASTComparisonOp.NotEqual,
            Keywords.IsLessThan => ASTComparisonOp.LessThan,
            Keywords.IsLessThanOrEqualTo => ASTComparisonOp.LessThanOrEqual,
            Keywords.IsGreaterThan => ASTComparisonOp.GreaterThan,
            Keywords.IsGreaterThanOrEqualTo => ASTComparisonOp.GreaterThanOrEqual,
            _ => throw new InvalidProgramException($"Invalid code reached in {nameof(TryParseComparison)}")
        };
        if (!TryParseExpression(tokens[..idxOp], out var leftOp))
        {
            Warning(nameof(TryParseComparison),
                "Attempted to parse comparison expression _ left operand parse failed.",
                [.. tokens]);
            return false;
        }
        if (!TryParseExpression(tokens[(idxOp + 1)..], out var rightOp))
        {
            Warning(nameof(TryParseComparison),
                "Attempted to parse comparison expression _ right operand parse failed.",
                [.. tokens]);
            return false;
        }
        output = new ASTComparison(tokens.GetASTPosition(), opCode, leftOp, rightOp);
        return true;
    }

    private bool TryParseBlock(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 2
            || tokens[0].Keyword != Keywords.BlockStart)
        {
            return false;
        }
        var innerTokens = tokens[1..];
        var nodes = new List<ASTNode>();
        if (!TryParseArray(nodes, innerTokens))
        {
            Error(nameof(TryParseBlock), "Error parsing block contents.", tokens);
        }
        output = new ASTBlock(tokens.GetASTPosition(), [.. nodes]);
        return true;
    }

    private bool TryParseParenthized(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        // First token
        if (tokens.Length < 2
            || tokens[0].Keyword != Keywords.OpenParen
            || tokens[^1].Keyword != Keywords.CloseParen)
            return false; // Fast fail no error.
        if (TryParseExpression(tokens[1..^1], out var inner))
        {
            output = new ASTParenthized(tokens.GetASTPosition(), inner);
            return true;
        }
        // Definitely parenthesis, definititely failed here.
        throw new ParserException(nameof(TryParseParenthized),
            "Could not parse parenthesis interior.",
            tokens);
    }

    private bool TryParseIf(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4) return false;
        if (tokens[0].IsNotKeyword(Keywords.If)) return false; // Fast fails no error
        int idxThen = tokens.NextTopLevelIndexOf(Keywords.Then);
        if (idxThen == -1)
        {
            // Its definitely an if and theres definity no 'then'
            Throw(nameof(TryParseIf), "Missing then from if statement.", tokens);
        }
        if (!TryParseExpression(tokens[1..idxThen], out var condition))
        {
            // Definitly an if expression parse failure
            Throw(nameof(TryParseIf), "Failed to parse if conditional expression.", tokens);
        }
        if (!TryParse(tokens[(idxThen + 1)..], out var body) ||
            body is not IStandalone)
        {
            // Definitely an if body parse failure.
            Throw(nameof(TryParseIf), "Failed to parse if body statement.", tokens);
        }
        output = new ASTIf(tokens.GetASTPosition(), (ASTExpression)condition!, body!);
        return true;
    }

    private bool TryParseIncrement(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 2) return false;
        if (tokens[0].Keyword != Keywords.Increment &&
            tokens[^1].Keyword != Keywords.Increment)
            return false;

        if (tokens[0].Keyword == Keywords.Increment)
        {
            if (!TryParseExpression(tokens[1..], out var expr))
                Throw(nameof(TryParseIncrement), "Error paring inner increment statement.", tokens);
            output = new ASTIncrement(tokens.GetASTPosition(), expr!, true);
            return true;
        }
        else
        {
            if (!TryParseExpression(tokens[..^1], out var expr))
                Throw(nameof(TryParseIncrement), "Error paring inner increment statement.", tokens);
            output = new ASTIncrement(tokens.GetASTPosition(), expr!, false);
            return true;
        }
    }

    private bool TryParseDecrement(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 2) return false;
        if (tokens[0].Keyword != Keywords.Decrement &&
            tokens[^1].Keyword != Keywords.Decrement)
            return false;

        if (tokens[0].Keyword == Keywords.Decrement)
        {
            if (!TryParseExpression(tokens[1..], out var expr))
                Throw(nameof(TryParseDecrement), "Error paring inner decrement expression.", tokens);
            output = new ASTDecrement(tokens.GetASTPosition(), expr!, true);
            return true;
        }
        else
        {
            if (!TryParseExpression(tokens[..^1], out var expr))
                Throw(nameof(TryParseDecrement), "Error paring inner decrement expression.", tokens);
            output = new ASTDecrement(tokens.GetASTPosition(), expr!, false);
            return true;
        }
    }

    private bool TryParseAssignment(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4) return false;
        if (tokens[0].IsNotKeyword(Keywords.Set) &&
            tokens[0].IsNotKeyword(Keywords.Assign) &&
            tokens[0].IsNotKeyword(Keywords.Put))
            return false;

        int idxSplit;
        if (tokens[0].IsKeyword(Keywords.Assign))
            idxSplit = tokens.NextTopLevelIndexOf(Keywords.Comma);
        else if (tokens[0].IsKeyword(Keywords.Set))
            idxSplit = tokens.NextTopLevelIndexOf(Keywords.EqualTo);
        else 
            idxSplit = tokens.NextTopLevelIndexOf(Keywords.Into);
        if (idxSplit == -1) return false; // Warn Here

        if (!TryParseExpression(tokens[1..idxSplit], out var objEx))
            return false;

        if (!TryParseExpression(tokens[(idxSplit + 1)..], out var valEx))
            return false;

        if (tokens[0].IsKeyword(Keywords.Put)) // Put Value Into Container Parsed in opposite positions.
        {
            output = new ASTAssignment(tokens.GetASTPosition(), valEx, objEx);
            return true;
        }
        output = new ASTAssignment(tokens.GetASTPosition(), objEx, valEx);
        return true;
    }

    private static bool TryParseLitChar(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].Type != TokenType.String)
            return false;
        var strVal = tokens[0].Content;
        if (strVal[0] == '\'' && strVal[^1] == '\'') strVal = strVal[1..^1]; // Remove only enclosing apostrophes.
        output = new ASTLitChar(tokens.GetASTPosition(), strVal);
        return true;
    }

    private static bool TryParseLitNull(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].Keyword != Keywords.Null)
            return false;
        output = new ASTLitNull(tokens.GetASTPosition());
        return true;
    }

    private static bool TryParseLitString(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].Type != TokenType.String)
            return false;
        var strVal = tokens[0].Content;
        if (strVal[0] == '"' && strVal[^1] == '"') strVal = strVal[1..^1]; // Remove only enclosing quotes.
        output = new ASTLitString(tokens.GetASTPosition(), strVal);
        return true;
    }

    private static bool TryParseLitNumber(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].Type != TokenType.Number)
            return false;
        output = new ASTLitNumber(tokens.GetASTPosition(), tokens[0].ConvertToWholeNumber());
        return true;
    }

    private static bool TryParseLitBool(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length != 1 ||
            (tokens[0].Keyword != Keywords.True && tokens[0].Keyword != Keywords.False))
            return false;
        output = new ASTLitBool(tokens.GetASTPosition(), tokens[0].Keyword == Keywords.True);
        return true;
    }

    private bool TryParseTypeInfo(CarmenToken[] tokens, [NotNullWhen(true)] out ASTTypeInfo? output)
    {
        output = null;
        if (tokens is null || tokens.Length == 0) return false;
        bool isNullable = false;
        bool isPointer = false;
        // Check and trim prefix.
        if (tokens[0].Keyword == Keywords.Nullable)
        {
            isNullable = true;
            tokens = tokens[1..];
        }
        else if (tokens[0].Keyword == Keywords.PointerTo)
        {
            isPointer = true;
            tokens = tokens[1..];
        }
        if (!tokens[0].TryMatchBaseType(out var ptype)) return false;
        if (ptype != Primitives.Array)
        {
            output = new ASTTypeInfo(tokens[0].Content, ptype, isNullable, isPointer, null);
            return true;
        }
        else
        {
            // Find of type keyword
            int idx_ofType = tokens.NextTopLevelIndexOf(Keywords.OfType);
            if (idx_ofType == -1) Throw(nameof(TryParseTypeInfo), "Error parsing array type info: missing type.", tokens);

            List<ASTExpression> dimensions = [];
            ASTTypeInfo? subType = null;
            if (tokens[1].Keyword == Keywords.WithSize)
            {
                var dimTok = tokens[2..idx_ofType].Split(Keywords.By);
                foreach (var dim in dimTok)
                {
                    if (!TryParseExpression(dim, out var dimex))
                        Throw(nameof(TryParseTypeInfo), "Error parsing array dimmension expression.", dim);
                    dimensions.Add((ASTExpression)dimex!);
                }
                //
                if (!TryParseTypeInfo(tokens[(idx_ofType + 1)..], out subType))
                    Throw(nameof(TryParseTypeInfo), "Error parsing array sub type expression.", tokens);
            }
            output = new(tokens[0].Content, ptype, isNullable, isPointer, new([..dimensions], subType!));
            return true;
        }
    }

    private bool TryParseVarDef(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        // ('define')? VARIABLE_ID 'is a' 
        // ( 'constant' VARIABLE_TYPES 'equal to' EXPRESSION
        // | ('static') ? VARIABLE_TYPES 'equal to' EXPRESSION )
        int idx_ofIsA = tokens.NextTopLevelIndexOf(Keywords.IsA);
        if ((idx_ofIsA != 1 && idx_ofIsA != 2)) { return false; }
        if (idx_ofIsA == 2 && tokens[0].IsNotKeyword(Keywords.Define)) { return false; }
        if (!TryParseIdentifier([tokens[(idx_ofIsA == 2 ? 1 : 0)]], out var identifier) || identifier is null)
            Throw(nameof(TryParseVarDef), "Error failed to parse definition identifier.", tokens);
        ASTVariableType defType = tokens[(idx_ofIsA == 2 ? 3 : 2)].Keyword switch
        {
            Keywords.Constant => ASTVariableType.Constant,
            Keywords.Static => ASTVariableType.Static,
            _ => ASTVariableType.Generic
        };
        int idx_ofAssign = tokens.NextTopLevelIndexOf(Keywords.EqualTo);
        ASTExpression? initial = null;
        if (idx_ofAssign != -1 && !TryParseExpression(tokens[(idx_ofAssign + 1)..], out initial))
            Throw(nameof(TryParseVarDef), "Error failed to parse definition initial value.", tokens);
        // Get type info
        int srt = (idx_ofIsA == 2 ? (defType != ASTVariableType.Generic ? 4 : 3) :
            (defType != ASTVariableType.Generic ? 3 : 2));
        if (!TryParseTypeInfo(
            (idx_ofAssign == -1 ? tokens[srt..] : tokens[srt..idx_ofAssign]),
            out var type) || type is null)
            Throw(nameof(TryParseVarDef), "Error failed to parse definition type.", tokens);
        output = new ASTVariableDefinition(tokens.GetASTPosition(), (ASTIdentifier)identifier!,
            defType, type!, initial!);
        return true;
    }

    private bool TryParseGoto(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 2
            || tokens[0].Keyword != Keywords.Goto
            || tokens[1].Type != TokenType.Identifier)
            return false;
        if (!TryParseIdentifier([tokens[1]], out var nId) || nId is not ASTIdentifier)
            Throw(nameof(TryParseGoto), "Error parsing goto statement identifier.", tokens);
        output = new ASTGoto(tokens.GetASTPosition(), (ASTIdentifier)nId!);
        return true;
    }

    private bool TryParseLabel(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 2
            || tokens[0].Keyword != Keywords.Label
            || tokens[1].Type != TokenType.Identifier)
            return false;
        if (!TryParseIdentifier([tokens[1]], out var nId) || nId is not ASTIdentifier)
            Throw(nameof(TryParseLabel), "Error parsing label statement identifier.", tokens);
        output = new ASTLabel(tokens.GetASTPosition(), (ASTIdentifier)nId!);
        return true;
    }

    private static bool TryParseIdentifier(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
    {
        output = null;
        if (tokens.Length != 1 || 
            tokens[0].Keyword != Keywords.Unknown ||
            tokens[0].Type != TokenType.Identifier) return false; // Keyword invalid id.
        output = new ASTIdentifier(tokens.GetASTPosition(), tokens[0].Content);
        return true;
    }

    private bool TryParseEPoint(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length <= 2 || tokens[0].IsNotKeyword(Keywords.Program))
            return false;

        if (!TryParse(tokens[1..], out var code))
        {
            Throw(nameof(TryParseEPoint), "Error parsing entry point statement.", tokens);
        }

        output = new ASTEntryPoint(tokens.GetASTPosition(), code!);
        return true;
    }
}
