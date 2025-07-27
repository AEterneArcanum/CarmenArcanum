using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;
using Arcane.Carmen.AST;
using Arcane.Carmen.Lexer;
using System.Diagnostics.CodeAnalysis;

namespace Arcane.Carmen.Parser;

public partial class CarmenParser
{
    private bool TryParseExpression(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        Debug($"Attempting to parse \"{tokens.AsString()}\" as an expression...");
        return TryParseParenthized(tokens, out output)
            || TryParseLitBool(tokens, out output)
            || TryParseLitNumber(tokens, out output)
            || TryParseLitString(tokens, out output)
            || TryParseLitChar(tokens, out output)
            || TryParseLitNull(tokens, out output)
            || TryParseIdentifier(tokens, out output)
            || TryParseComparison(tokens, out output)
            || TryParseDecrement(tokens, out output)
            || TryParseIncrement(tokens, out output);
    }

    private bool TryParse(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
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
            return TryParseExpression(tokens, out output);
        }
        catch (ParserException ex)
        {
            Error(ex.Function, ex.Message, [.. ex.Tokens]);
        }
        output = null;
        return false;
    }

    private bool TryParseComparison(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
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
        int idxOp = Utils.NextTopLevelIndexOfKeywords(tokens, opwords, out var match);
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
        if (!TryParseExpression(tokens[..idxOp], out var leftOp)
            || leftOp is not ASTExpression lex)
        {
            Warning(nameof(TryParseComparison),
                "Attempted to parse comparison expression _ left operand parse failed.",
                [.. tokens]);
            return false;
        }
        if (!TryParseExpression(tokens[(idxOp + 1)..], out var rightOp)
            || rightOp is not ASTExpression rex)
        {
            Warning(nameof(TryParseComparison),
                "Attempted to parse comparison expression _ right operand parse failed.",
                [.. tokens]);
            return false;
        }
        output = new ASTComparison(tokens.GetASTPosition(), opCode, lex, rex);
        return true;
    }

    private bool TryParseBlock(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 2
            || !Utils.IsKeyword(tokens[0], Keywords.BlockStart))
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

    private bool TryParseParenthized(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        // First token
        if (tokens.Length < 2
            || !Utils.IsKeyword(tokens[0], Keywords.OpenParen)
            || !Utils.IsKeyword(tokens[^1], Keywords.CloseParen))
            return false; // Fast fail no error.
        if (TryParseExpression(tokens[1..^1], out var inner)
            && inner is ASTExpression expr)
        {
            output = new ASTParenthized(tokens.GetASTPosition(), expr);
            return true;
        }
        // Definitely parenthesis, definititely failed here.
        throw new ParserException(nameof(TryParseParenthized),
            "Could not parse parenthesis interior.",
            tokens);
    }

    private bool TryParseIf(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4) return false;
        if (!Utils.IsKeyword(tokens[0], Keywords.If)) return false; // Fast fails no error
        int idxThen = Utils.NextTopLevelIndexOfKeyword(tokens, Keywords.Then);
        if (idxThen == -1)
        {
            // Its definitely an if and theres definity no 'then'
            Throw(nameof(TryParseIf), "Missing then from if statement.", tokens);
        }
        if (!TryParseExpression(tokens[1..idxThen], out var condition) ||
            condition is not ASTExpression)
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

    private bool TryParseIncrement(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 2) return false;
        if (!Utils.IsKeyword(tokens[0], Keywords.Increment) &&
            !Utils.IsKeyword(tokens[^1], Keywords.Increment))
            return false;

        if (Utils.IsKeyword(tokens[0], Keywords.Increment))
        {
            if (!TryParseExpression(tokens[1..], out var expr))
                Throw(nameof(TryParseIncrement), "Error paring inner increment statement.", tokens);
            output = new ASTIncrement(tokens.GetASTPosition(), (ASTExpression)expr!, true);
            return true;
        }
        else
        {
            if (!TryParseExpression(tokens[..^1], out var expr))
                Throw(nameof(TryParseIncrement), "Error paring inner increment statement.", tokens);
            output = new ASTIncrement(tokens.GetASTPosition(), (ASTExpression)expr!, false);
            return true;
        }
    }

    private bool TryParseDecrement(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 2) return false;
        if (!Utils.IsKeyword(tokens[0], Keywords.Decrement) &&
            !Utils.IsKeyword(tokens[^1], Keywords.Decrement))
            return false;

        if (Utils.IsKeyword(tokens[0], Keywords.Decrement))
        {
            if (!TryParseExpression(tokens[1..], out var expr))
                Throw(nameof(TryParseDecrement), "Error paring inner decrement expression.", tokens);
            output = new ASTDecrement(tokens.GetASTPosition(), (ASTExpression)expr!, true);
            return true;
        }
        else
        {
            if (!TryParseExpression(tokens[..^1], out var expr))
                Throw(nameof(TryParseDecrement), "Error paring inner decrement expression.", tokens);
            output = new ASTDecrement(tokens.GetASTPosition(), (ASTExpression)expr!, false);
            return true;
        }
    }

    private bool TryParseAssignment(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 5
            || !Utils.IsKeyword(tokens[0], Keywords.Set))
            return false; // Fast fail

        // Get equal index
        int idxEqual = Utils.NextTopLevelIndexOfKeyword(tokens, Keywords.Equal);
        if (idxEqual == -1) Throw(nameof(TryParseAssignment), "Missing 'equal' from assignment statement.", tokens);
        // Check to following equal.
        int idxTo = (tokens[idxEqual + 1].TryMatchKeyword(out var wto) ? (wto == Keywords.To ? idxEqual + 1 : -1) : -1);
        if (idxTo == -1) Throw(nameof(TryParseAssignment), "Missing 'to' from assignment statement.", tokens);
        // expression from 'set' to 'equal'
        if (!TryParseExpression(tokens[1..idxEqual], out var objEx) ||
            objEx is not ASTExpression) Throw(nameof(TryParseAssignment), "Error parsing object expression in assignment statement.", tokens);
        // expression after 'to'
        if (!TryParseExpression(tokens[(idxTo + 1)..], out var valEx) ||
            valEx is not ASTExpression) Throw(nameof(TryParseAssignment), "Error parsing value expression in assignment statement.", tokens);
        output = new ASTAssignment(tokens.GetASTPosition(), (ASTExpression)objEx!, (ASTExpression)valEx!);
        return true;
    }

    private static bool TryParseLitChar(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].Type != TokenType.String)
            return false;
        var strVal = tokens[0].Content;
        if (strVal[0] == '\'' && strVal[^1] == '\'') strVal = strVal[1..^1]; // Remove only enclosing apostrophes.
        output = new ASTLitChar(tokens.GetASTPosition(), strVal);
        return true;
    }

    private static bool TryParseLitNull(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1 || !Utils.IsKeyword(tokens[0], Keywords.Null))
            return false;
        output = new ASTLitNull(tokens.GetASTPosition());
        return true;
    }

    private static bool TryParseLitString(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].Type != TokenType.String)
            return false;
        var strVal = tokens[0].Content;
        if (strVal[0] == '"' && strVal[^1] == '"') strVal = strVal[1..^1]; // Remove only enclosing quotes.
        output = new ASTLitString(tokens.GetASTPosition(), strVal);
        return true;
    }

    private static bool TryParseLitNumber(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].Type != TokenType.Number)
            return false;
        output = new ASTLitNumber(tokens.GetASTPosition(), tokens[0].ConvertToWholeNumber());
        return true;
    }

    private static bool TryParseLitBool(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1
            || !Utils.IsKeyword(tokens[0], [Keywords.True, Keywords.False], out var match))
            return false;
        output = new ASTLitBool(tokens.GetASTPosition(), match == Keywords.True);
        return true;
    }

    private bool TryParseVarDef(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4
            || !Utils.IsKeyword(tokens[1], Keywords.Is))
            return false;
        if (!TryParseIdentifier([tokens[0]], out var nId) || nId is not ASTIdentifier identity)
            return false;
        bool isArray = !Utils.IsKeyword(tokens[2], Keywords.A);
        int arraySize = -1; // -1 not an array
        if (isArray)
        {
            if (!(tokens[2].Type == TokenType.Number)) return false;
            arraySize = (int)tokens[2].ConvertToWholeNumber();
        }

        // Future tokens 3 may be constant or static
        int idx = 3; // Future Use For push on 3 being stat/const/pub/pri

        if (tokens[idx].TryMatchKeyword(out _)) return false; // Shouldnt be a keyword
        tokens[idx].TryMatchBaseType(out var wB); // Get base type
        string typeId = tokens[idx].Content;
        ASTTypeInfo info = new(typeId, wB, arraySize);


        output = new ASTVariableDefinition(tokens.GetASTPosition(), identity, info);
        return true;
    }

    private bool TryParseGoto(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 2
            || !Utils.IsKeyword(tokens[0], Keywords.Goto)
            || tokens[1].Type != TokenType.Identifier)
            return false;
        if (!TryParseIdentifier([tokens[1]], out var nId) || nId is not ASTIdentifier)
            Throw(nameof(TryParseGoto), "Error parsing goto statement identifier.", tokens);
        output = new ASTGoto(tokens.GetASTPosition(), (ASTIdentifier)nId!);
        return true;
    }

    private bool TryParseLabel(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 2
            || !Utils.IsKeyword(tokens[0], Keywords.Label)
            || tokens[1].Type != TokenType.Identifier)
            return false;
        if (!TryParseIdentifier([tokens[1]], out var nId) || nId is not ASTIdentifier)
            Throw(nameof(TryParseLabel), "Error parsing label statement identifier.", tokens);
        output = new ASTLabel(tokens.GetASTPosition(), (ASTIdentifier)nId!);
        return true;
    }

    private static bool TryParseIdentifier(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].TryMatchKeyword(out var _)) return false; // Keyword invalid id.
        output = new ASTIdentifier(tokens.GetASTPosition(), tokens[0].Content);
        return true;
    }

    private bool TryParseEPoint(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length <= 2
            || !Utils.IsKeyword(tokens[0], Keywords.Program))
            return false;

        if (!TryParse(tokens[1..], out var code))
        {
            Throw(nameof(TryParseEPoint), "Error parsing entry point statement.", tokens);
        }
        output = new ASTEntryPoint(tokens.GetASTPosition(), code!);
        return true;
    }
}
