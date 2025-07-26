using Arcane.Carmen.AST;
using Arcane.Carmen.AST.Types;
using Arcane.Carmen.Lexer;
using System.Diagnostics.CodeAnalysis;

namespace Arcane.Carmen.Parser;

public enum LogLevel
{
    Noise,
    Regular,
    Warning,
    Error
}
public record ParserLogEntry(DateTime Time, LogLevel Level, string Log);
public record ParserError(string Function, string message, Token[] Tokens);

public class ParserException( string function, string message, 
    IReadOnlyCollection<Token> tokens) 
    : Exception(message)
{
    public string Function { get; init; } = function;
    public IReadOnlyCollection<Token> Tokens { get; init; } = tokens;
}

public class CarmenParser
{
    public event Action<ParserLogEntry>? OnParserLog;
    public event Action<ParserError>? OnParserError;

    public readonly List<ParserLogEntry> LogEntries = [];
    public readonly List<ParserError> ParserErrors = [];
    public readonly List<ASTNode> ParsedNodes = [];

    private void Throw(string src, string message, IEnumerable<Token> tokens)
    {
        throw new ParserException(src, message, (IReadOnlyCollection<Token>)tokens);
    }

    private void Log(string content, LogLevel level = LogLevel.Regular)
    {
        LogEntries.Add(new(DateTime.Now, level, content));
        OnParserLog?.Invoke(LogEntries.Last());
    }

    private void Error(string src, string msg, Token[] tks) 
    {
        Log(msg, LogLevel.Error);
        ParserErrors.Add(new(src, msg, tks));
        OnParserError?.Invoke(ParserErrors.Last());
    }

    private void Warning(string src, string msg, Token[] tks)
    {
        Log($"{src}::{msg}", LogLevel.Warning);
    }

    public void Reset()
    {
        LogEntries.Clear();
        ParserErrors.Clear();
        ParsedNodes.Clear();
    }

    public void ParseTokens(Token[] tokens)
    {
        Log("Parsing tokens...");
        if (tokens is null || tokens.Length == 0)
        {
            Log("No tokens passed returning.");
            return;
        }
        Log($"{tokens.Length} passed.");
        Log("Clearing comments and empty tokens from the passed tokens.");
        var wrktk = new List<Token>();
        foreach (var token in tokens)
        {
            if (token is not null &&
                token.Content.Length != 0 &&
                token.Type != TokenType.Comment &&
                token.Type != TokenType.Whitespace)
                wrktk.Add(token);
        }
        Log($"{wrktk.Count} tokens remain.");
        if (wrktk.Count == 0)
        {
            Log("Returning.");
            return;
        }

        Log("Iterating tokens.");
        TryParseArray(ParsedNodes, [..wrktk]);

        Log($"Attempt complete... {ParserErrors.Count} Errors...");
    }

    private bool TryParseArray(List<ASTNode> Collection, Token[] tokens)
    {
        int lst = 0; // last ep
        int depth = 0; // block depth
        for (int i = 0; i < tokens.Length; i++)
        {
            var token = tokens[i];
            Log($"Targeting node {i}:{token.Content}", LogLevel.Noise);
            if (!token.TryMatchKeyword(out var keyword))
            {
                Log($"Node {i}:{token.Content} is not a key token.", LogLevel.Noise);
                continue;
            }

            switch (keyword)
            {
                case Keywords.OpenParen:
                case Keywords.BlockStart:
                    Log($"Node {i}:{token.Content} is opening a block.", LogLevel.Noise);
                    depth++;
                    break;
                case Keywords.CloseParen:
                case Keywords.BlockEnd:
                    Log($"Node {i}:{token.Content} is closing a block.", LogLevel.Noise);
                    depth--;
                    break;
            }

            if (depth == 0 && (
                keyword == Keywords.EOS ||
                keyword == Keywords.BlockEnd))
            {
                Log($"Level zero terminator found at {i}.", LogLevel.Noise);
                var tok = tokens[lst..i];
                if (tok.Length != 0)
                {
                    // try parse tok and add to nodes
                    if (TryParse(tok, out var node))
                    {
                        Collection.Add(node);
                        Log($"Parsed node {node}.");
                    }
                    lst = i + 1;
                }
            }
        }
        return true;
    }

    private bool TryParseExpression(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        Log($"Attempting to parse \"{tokens.AsString()}\" as an expression...");
        return TryParseParenthized(tokens, out output)
            || TryParseLitBool(tokens, out output)
            || TryParseLitNumber(tokens, out output)
            || TryParseIdentifier(tokens, out output)
            || TryParseComparison(tokens, out output)
            || TryParseDecrement(tokens, out output)
            || TryParseIncrement(tokens, out output);
    }

    private bool TryParse(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        try
        {
            Log($"Attempting to parse \"{tokens.AsString()}\" as a statement...");
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
            Error(ex.Function, ex.Message, [..ex.Tokens]);
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
        output = new ASTComparison(tokens.GetPosition(), opCode, lex, rex);
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
        output = new ASTBlock(tokens.GetPosition(), [.. nodes]);
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
            output = new ASTParenthized(tokens.GetPosition(), expr);
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
        output = new ASTIf(tokens.GetPosition(), (ASTExpression)condition!, body!);
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
            output = new ASTIncrement(tokens.GetPosition(), (ASTExpression)expr!, true);
            return true;
        }
        else
        {
            if (!TryParseExpression(tokens[..^1], out var expr))
                Throw(nameof(TryParseIncrement), "Error paring inner increment statement.", tokens);
            output = new ASTIncrement(tokens.GetPosition(), (ASTExpression)expr!, false);
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
            output = new ASTDecrement(tokens.GetPosition(), (ASTExpression)expr!, true);
            return true;
        }
        else
        {
            if (!TryParseExpression(tokens[..^1], out var expr))
                Throw(nameof(TryParseDecrement), "Error paring inner decrement expression.", tokens);
            output = new ASTDecrement(tokens.GetPosition(), (ASTExpression)expr!, false);
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
        output = new ASTAssignment(tokens.GetPosition(), (ASTExpression)objEx!, (ASTExpression)valEx!);
        return true;
    }

    private bool TryParseLitNumber(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].Type != TokenType.Number)
            return false;
        output = new ASTLitNumber(tokens.GetPosition(), tokens[0].ConvertToWholeNumber());
        return true;
    }

    private bool TryParseLitBool(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1
            || !Utils.IsKeyword(tokens[0], [Keywords.True, Keywords.False], out var match))
            return false;
        output = new ASTLitBool(tokens.GetPosition(), match == Keywords.True);
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


        output = new ASTVariableDefinition(tokens.GetPosition(), identity, info);
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
        output = new ASTGoto(tokens.GetPosition(), (ASTIdentifier)nId!);
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
        output = new ASTLabel(tokens.GetPosition(), (ASTIdentifier)nId!);
        return true;
    }

    private bool TryParseIdentifier(Token[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].TryMatchKeyword(out var _)) return false; // Keyword invalid id.
        output = new ASTIdentifier(tokens.GetPosition(), tokens[0].Content);
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
        output = new ASTEntryPoint(tokens.GetPosition(), code!);
        return true;
    }
}
