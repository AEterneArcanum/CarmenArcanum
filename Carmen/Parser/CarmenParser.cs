using Arcane.Carmen.AST;
using Arcane.Carmen.Lexer;
using Arcane.Carmen.Logging;
using System.Runtime.CompilerServices;

namespace Arcane.Carmen.Parser;

public partial class CarmenParser
{
    public event Action<ParserError>? OnParserError;

    public readonly ParserLog LogBook = new();
    public readonly List<ParserError> ParserErrors = [];
    public readonly List<ASTNode> ParsedNodes = [];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Log(string content, LogLevel level = LogLevel.Info)
    {
        LogBook.Log(content, level);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Debug(string content)
    {
        Log(content, LogLevel.Debug);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Error(string src, string msg, Token[] tks) 
    {
        Log(msg, LogLevel.Error);
        ParserErrors.Add(new(src, msg, tks));
        OnParserError?.Invoke(ParserErrors.Last());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Warning(string src, string msg, Token[] tks)
    {
        Log($"{src}::{msg}", LogLevel.Warn);
    }

    public void Reset()
    {
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
        
        Log("Preprocessing tokens.");
        var wrktk = Preprocess([.. tokens]);
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

    private bool TryParseArray(List<ASTNode> Collection, CarmenToken[] tokens)
    {
        int lst = 0; // last ep
        int depth = 0; // block depth

        bool inDo = false;

        for (int i = 0; i < tokens.Length; i++)
        {
            var token = tokens[i];
            Log($"Targeting node {i}:{token.Content}", LogLevel.Debug);
            if (!token.TryMatchKeyword(out var keyword))
            {
                Debug($"Node {i}:{token.Content} is not a key token.");
                continue;
            }

            // Should catch as we move into as statement
            if (depth == 0 &&
                keyword == Keywords.Do)
            {
                // Is ;do or }do ... not end of while...do
                if (i == 0 || 
                    (i > 0 && 
                    (tokens[i - 1].IsKeyword(Keywords.EOS) ||
                    tokens[i - 1].IsKeyword(Keywords.BlockEnd))))
                {
                    inDo = true;
                    continue;
                }
            } 

            switch (keyword)
            {
                case Keywords.OpenParen:
                case Keywords.BlockStart:
                    Debug($"Node {i}:{token.Content} is opening a block.");
                    depth++;
                    break;
                case Keywords.CloseParen:
                case Keywords.BlockEnd:
                    Debug($"Node {i}:{token.Content} is closing a block.");
                    depth--;
                    break;
            }

            if (depth == 0 && (
                keyword == Keywords.EOS ||
                keyword == Keywords.BlockEnd))
            {
                if (keyword == Keywords.BlockEnd &&
                    inDo && i + 1 < tokens.Length &&
                    tokens[i + 1].IsKeyword(Keywords.While))
                {
                    // End of do block continue to end of while expression
                    inDo = false; 
                    continue;
                }

                Debug($"Level zero terminator found at {i}.");
                var tok = tokens[lst..i];
                if (tok.Length != 0)
                {
                    // try parse tok and add to nodes
                    if (TryParse(tok, out var node))
                    {
                        Collection.Add(node);
                        Log($"Parsed node {node.GetType()}.");
                    }
                    lst = i + 1;
                }
            }
        }
        return true;
    }
}
