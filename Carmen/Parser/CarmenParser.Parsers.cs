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
            return TryParseBlock(tokens, out output)
                || TryParseBreak(tokens, out output)
                || TryParseContinue(tokens, out output)
                || TryParseIf(tokens, out output)
                || TryParseAssignment(tokens, out output)
                || TryParseVarDef(tokens, out output)
                || TryParseCompoundAssign(tokens, out output)
                || TryParseGoto(tokens, out output)
                || TryParseLabel(tokens, out output)
                || TryParseEnum(tokens, out output)
                || TryParseEPoint(tokens, out output)
                || TryParseImport(tokens, out output)
                || TryParseIterate(tokens, out output)
                || TryParseASM(tokens, out output)
                || TryParseForEach(tokens, out output)
                || TryParseFor(tokens, out output)
                || TryParseCase(tokens, out output)
                || TryParseArchDefined(tokens, out output)
                || TryParseReturn(tokens, out output)
                || TryParseStructure(tokens, out output)
                || TryParseAssert(tokens, out output)
                || TryParseSwitch(tokens, out output)
                || TryParseFunction(tokens, out output)
                || TryParseDoWhile(tokens, out output)
                || TryParseLoop(tokens, out output)
                || TryParseDecrement(tokens, out output)
                || TryParseFunctionCall(tokens, out output)
                || TryParseIncrement(tokens, out output);
        }
        catch (ParserException ex)
        {
            Error(ex.Function, ex.Message, [.. ex.Tokens]);
        }
        output = null;
        return false;
    }

    private bool TryParseArchDefined(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 2 || (
            tokens[0].IsNotKeyword(Keywords.Arch16) &&
            tokens[0].IsNotKeyword(Keywords.Arch32) &&
            tokens[0].IsNotKeyword(Keywords.Arch64))) return false;
        ASTArchitecture arch = tokens[0].Keyword switch
        {
            Keywords.Arch16 => ASTArchitecture.Arch16,
            Keywords.Arch32 => ASTArchitecture.Arch32,
            Keywords.Arch64 => ASTArchitecture.Arch64,
            _ => throw new NotImplementedException("Not yet implemented architecture.")
        };
        if (!TryParse(tokens[1..], out var statement)) return false;
        output = new ASTArchDefined(tokens.GetASTPosition(), arch, (ASTStatement)statement);
        return true;
    }

    private bool TryParseFunction(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4
            || (tokens[0].IsNotKeyword(Keywords.Inline)
            && tokens[0].IsNotKeyword(Keywords.Function)))
        {
            return false;
        }
        int idxBS = tokens.NextTopLevelIndexOf(Keywords.BlockStart);
        if (idxBS == -1) { return false; }

        int idx = 0;

        bool inline = false;
        if (tokens[idx].IsKeyword(Keywords.Inline))
        {
            inline = true;
            idx++;
        }

        ASTArchitecture? arch = null;
        if (tokens[idx].IsKeyword(Keywords.Arch16))
        {
            arch = ASTArchitecture.Arch16;
            idx++;
        }
        else if (tokens[idx].IsKeyword(Keywords.Arch32))
        {
            arch = ASTArchitecture.Arch32;
            idx++;
        }
        else if (tokens[idx].IsKeyword(Keywords.Arch64))
        {
            arch = ASTArchitecture.Arch64;
            idx++;
        }

        if (tokens[idx].IsNotKeyword(Keywords.Function))
        {
            return false;
        }
        idx++;

        if (!TryParseIdentifier([tokens[idx]], out var identifier))
        {
            return false;
        }
        idx++;

        // Look for For, Returning, And With
        int idxFor = tokens.NextTopLevelIndexOf(Keywords.For);
        if (idxFor != -1 && idxFor != idx)
        {
            return false; // Basd Placement
        }
        if (!TryParseIdentifier([tokens[idxFor + 1]], out var structId))
        {
            return false;
        }
        else
        {
            idx += 2; // For defined parse success should be idx of next part
        }

        int idxReturn = tokens.NextTopLevelIndexOf(Keywords.Returning);
        if (idxReturn != -1 && idxReturn != idx)
        {
            return false;
        }

        int idxWith = tokens.NextTopLevelIndexOf(Keywords.With);

        ASTTypeInfo? retType = null;
        if (idxReturn != -1 && idxWith != -1)
        {
            if (!TryParseTypeInfo(tokens[(idxReturn + 1)..idxWith],
                out retType))
            {
                return false;
            }
        }
        else if (idxReturn != -1)
        {
            if (!TryParseTypeInfo(tokens[(idxReturn + 1).. idxBS],
                out retType))
            {
                return false;
            }
        }

        List<ASTFuncParamDecl> Params = [];
        if (idxWith != -1)
        {
            var parts = tokens[(idxWith + 1)..].Split(Keywords.Comma);
            foreach (var part in parts)
            {
                if (!TryParseFunctionParamDecl(part, out var param))
                {
                    return false;
                }
                Params.Add(param);
            }
        }

        if (!TryParseBlock(tokens[idxBS..], out var block))
        {
            return false;
        }

        output = new ASTFunction(tokens.GetASTPosition(),
            (ASTIdentifier)identifier,
            inline,
            arch,
            (ASTIdentifier)structId,
            retType,
            [.. Params],
            (ASTBlock)block);
        return true;
    }

    private bool TryParseStructure(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output) {
        output = null;
        if (tokens.Length < 4 || tokens[0].IsNotKeyword(Keywords.Structure))
            return false;
        if (!TryParseIdentifier(tokens[1..2], out var iden))
            return false;
        if (!TryParseBlock(tokens[2..], out var body))
            return false;
        output = new ASTStructure(tokens.GetASTPosition(), 
            (ASTIdentifier)iden, (ASTBlock)body);
        return true;
    }

    private bool TryParseCase(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 2 ||
            tokens[0].IsNotKeyword(Keywords.Case))
            return false;

        if (!TryParseExpression(tokens[1..], out var expression)) 
            return false;

        output = new ASTCase(tokens.GetASTPosition(), expression);
        return true;
    }

    private bool TryParseSwitch(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4 || tokens[0].IsNotKeyword(Keywords.Switch))
            return false;

        int idxBlk = tokens.NextTopLevelIndexOf(Keywords.BlockStart);
        if (idxBlk == -1) return false;

        if (!TryParseExpression(tokens[1..idxBlk], out var expression)) return false;
        if (!TryParseBlock(tokens[idxBlk..], out var block)) return false;

        output = new ASTSwitch(tokens.GetASTPosition(), expression, (ASTBlock)block);
        return true;
    }
    /// <summary>
    /// Attempt to parse an inline assembly statement.
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private bool TryParseASM(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 2 || tokens[0].IsNotKeyword(Keywords.Asm))
            return false;
        int p = 1;
        bool isunsafe = false;
        if (tokens[p].IsKeyword(Keywords.Unsafe))
        {
            isunsafe = true;
            p++;
        }
        if (tokens[p].IsNotKeyword(Keywords.Arch16)
            && tokens[p].IsNotKeyword(Keywords.Arch32)
            && tokens[p].IsNotKeyword(Keywords.Arch64))
        {
            return false;
        }
        ASTArchitecture arch = tokens[p].Keyword switch
        {
            Keywords.Arch16 => ASTArchitecture.Arch16,
            Keywords.Arch32 => ASTArchitecture.Arch32,
            Keywords.Arch64 => ASTArchitecture.Arch64,
            _ => throw new InvalidOperationException()
        };
        p++;
        ASTExpression? clobbers = null;
        if (tokens[p].IsKeyword(Keywords.Clobbers))
        {
            if (tokens[p+1].Type != TokenType.String ||
                !TryParseLitString(tokens[(p + 1)..(p + 2)],
                out clobbers)
                || clobbers is not ASTLitString)
            {
                return false;
            }
            p += 2;
        }

        if (!TryParseExpression(tokens[p..], out var code) 
            || code is not ASTLitString)
        {
            return false;
        }

        output = new ASTASM(tokens.GetASTPosition(), isunsafe, arch,
            (ASTLitString?)clobbers, (ASTLitString)code);
        return true;
    }
    /// <summary>
    /// Attempt to parse an assert statement. 
    /// "Message" if Condition.
    /// ('assert' EXPRESSION if EXPRESSION)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseAssert(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4
            || tokens[0].IsNotKeyword(Keywords.Assert))
            return false;
        // Get IF
        int idxIf = tokens.NextTopLevelIndexOf(Keywords.If);
        if (idxIf < 2 || idxIf > tokens.Length - 2)
            return false;

        if (!TryParseExpression(tokens[1..idxIf], out var message))
            return false;

        if (!TryParseExpression(tokens[(idxIf + 1)..], out var condition))
            return false;

        output = new ASTAssert(tokens.GetASTPosition(), condition, message);
        return true;
    }
    /// <summary>
    /// Attempt to parse a return statement.
    /// ('return' [EXPRESSION]?)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseReturn(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 1 || tokens[0].IsNotKeyword(Keywords.Return))
            return false;

        if (tokens.Length ==1)
        {
            output = new ASTReturn(tokens.GetASTPosition(), null);
            return true;
        }

        if (!TryParseExpression(tokens[1..], out var ret))
            { return false; }
        output = new ASTReturn(tokens.GetASTPosition(), ret);
        return true;
    }
    /// <summary>
    /// Attempt to parse an enumeration.
    /// ('enumerate' IDENTIFIER 'with' IDENTIFIER [',' IDENTIFIER]*)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseEnum(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4 
            || tokens[0].IsNotKeyword(Keywords.Enumerate))
            return false;
        if (tokens[2].IsNotKeyword(Keywords.With))
            return false;

        if (!TryParseIdentifier(tokens[1..2], out var enumiden))
            return false;

        List<ASTIdentifier> enumerations = [];

        foreach (var item in tokens[3..].Split(Keywords.Comma))
        {
            if (!TryParseIdentifier(item, out var identifier))
            {
                continue;
            }
            enumerations.Add((ASTIdentifier)identifier);
        }

        output = new ASTEnumDefinition(tokens.GetASTPosition(), (ASTIdentifier)enumiden, [..enumerations]);
        return true;
    }
    /// <summary>
    /// Attempt to parse an iterate statement.
    /// ('iterate over' EXPRESSION 'do' BLOCK)
    /// Compilation manages 'index' and 'value' references.
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseIterate(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4 || tokens[0].IsNotKeyword(Keywords.IterateOver))
            return false;
        int idxDo = tokens.NextTopLevelIndexOf(Keywords.Do);
        if (idxDo < 2)
            return false;
        if (!TryParseExpression(tokens[1..idxDo], out var objex))
            return false;
        if (!TryParseBlock(tokens[(idxDo + 1)..], out var block))
            return false;
        output = new ASTIterator(tokens.GetASTPosition(), objex, (ASTBlock)block);
        return true;
    }
    /// <summary>
    /// Attempt to parse a for each loop.
    /// ('for each' IDENTIFIER 'in' EXPRESSION 'do' BLOCK)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseForEach(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;

        if (tokens.Length < 6 
            || tokens[0].IsNotKeyword(Keywords.ForEach) 
            || tokens[2].IsNotKeyword(Keywords.In)) 
            return false;

        int idxDo = tokens.NextTopLevelIndexOf(Keywords.Do);
        if (idxDo == -1) return false;

        if (!TryParseIdentifier(tokens[1..2], out var iden))
            return false;

        if (!TryParseExpression(tokens[3..idxDo], out var objex))
            return false;

        if (!TryParseBlock(tokens[(idxDo+1)..], out var block))
            return false;

        output = new ASTForEach(tokens.GetASTPosition(), (ASTIdentifier)iden, objex, (ASTBlock)block);
        return true;
    }
    /// <summary>
    /// Attempt to parse a for loop.
    /// ('for' IDENTIFIER 'from' EXPRESSION ['while'|'until'] EXPRESSION ['step' EXPRESSION]? 'do' BLOCK)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseFor(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 7 || tokens[0].IsNotKeyword(Keywords.For) ||
            tokens[2].IsNotKeyword(Keywords.From)) return false;
        if (!TryParseIdentifier(tokens[1..2], out var iden)) { return false; }

        int idxDo = tokens.NextTopLevelIndexOf(Keywords.Do); // Occurs just before the block
        if (idxDo < 6) return false;

        // idxFrom == 2
        int idxcon = tokens.NextTopLevelIndexOf(
            [Keywords.While, Keywords.Until],
            out var match);
        if (idxcon < 4 || idxcon > idxDo - 2) return false;
        bool isUntil = match == Keywords.Until;

        // Check for step expression
        int idxStep = tokens.NextTopLevelIndexOf(Keywords.Step);
        if (idxStep != -1 && (idxStep < idxcon + 2 || idxStep > idxDo - 2)) return false;

        ASTExpression? stepex = null;
        if (idxStep != -1 && !TryParseExpression(tokens[(idxStep + 1)..idxDo], out stepex))
            return false;

        // Get condition expression
        if (!TryParseExpression(tokens[(idxcon + 1)..((idxStep != -1) ? idxStep : idxDo)], out var conex)) 
            { return false; }

        // Get start expression
        if (!TryParseExpression(tokens[3..idxcon], out var startexpr)) { return false; }

        // Get the block
        if (!TryParseBlock(tokens[(idxDo + 1)..], out var block)) { return false; }

        output = new ASTFor(tokens.GetASTPosition(), (ASTIdentifier)iden, startexpr, conex, isUntil, stepex, (ASTBlock)block);
        return true;
    }
    /// <summary>
    /// Attempt to parse a do/while loop.
    /// (('do' BLOCK 'while'|'until' EXPRESSION) | 
    /// ('while'|'until' EXPRESSION) 'do' BLOCK)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseDoWhile(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4 || (tokens[0].IsNotKeyword(Keywords.Do) && tokens[0].IsNotKeyword(Keywords.While))) return false;
        bool Prefix = tokens[0].IsKeyword(Keywords.While) || tokens[0].IsKeyword(Keywords.Until);
        int idxBreak = Prefix ? 
            tokens.NextTopLevelIndexOf(Keywords.Do) :
            tokens.NextTopLevelIndexOf(Keywords.BlockEnd);
        if (!Prefix && (tokens[idxBreak + 1].IsNotKeyword(Keywords.While) ||
            tokens[idxBreak + 1].IsNotKeyword(Keywords.Until))) return false;

        bool isUntil = Prefix ?
            tokens[0].IsKeyword(Keywords.Until) :
            tokens[idxBreak + 1].IsKeyword(Keywords.Until);

           // tokens.NextTopLevelIndexOf(Prefix ? Keywords.Do : Keywords.While);
        if (Prefix)
        {
            if (!TryParseExpression(tokens[1..idxBreak], out var cond)) return false;
            if (!TryParseBlock(tokens[(idxBreak + 1)..],  out var block)) return false;
            output = new ASTDoWhile(tokens.GetASTPosition(), cond, (ASTBlock)block, isUntil, Prefix);
            return true;
        }
        else
        {
            if (!TryParseBlock(tokens[1..idxBreak], out var block)) return false;
            if (!TryParseExpression(tokens[(idxBreak + 2)..], out var cond)) return false;
            output = new ASTDoWhile(tokens.GetASTPosition(), cond, (ASTBlock)block, isUntil, Prefix);
            return true;
        }
    }
    /// <summary>
    /// Attempt to parse a loop statement.
    /// ('loop' BLOCK)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseLoop(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 3 || tokens[0].IsNotKeyword(Keywords.Loop)) return false;
        if (!TryParseBlock(tokens[1..], out var block)) return false;
        output = new ASTLoop(tokens.GetASTPosition(), (ASTBlock)block);
        return true;
    }

    private static bool TryParseImport(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 2 || tokens[0].IsNotKeyword(Keywords.Import)) return false;
        if (tokens[1].Type != TokenType.String) return false;

        if (!TryParseLitString(tokens[1..], out var str)) return false;

        output = new ASTImport(tokens.GetASTPosition(), (ASTLitString)str);
        return true;
    }

    private bool TryParseCompoundAssign(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 3) return false;
        Keywords[] operators = [
            Keywords.Add,
            Keywords.Subtract,
            Keywords.Multiply,
            Keywords.Divide,
            Keywords.Modulate,
            Keywords.Concatenate,
            Keywords.CompoundAdd,
            Keywords.CompoundSubtract,
            Keywords.CompoundMultiply,
            Keywords.CompoundDivide,
            Keywords.CompoundModulate,
            Keywords.CompoundConcatenate,
            ];
        int idxOp = tokens.NextTopLevelIndexOf(operators, out var mat);
        if (idxOp == -1) return false;

        ASTCompoundOp op = mat switch
        {
            Keywords.Add or
            Keywords.CompoundAdd => ASTCompoundOp.Add,
            Keywords.Subtract or
            Keywords.CompoundSubtract => ASTCompoundOp.Subtract,
            Keywords.Multiply or
            Keywords.CompoundMultiply => ASTCompoundOp.Multiply,
            Keywords.Divide or
            Keywords.CompoundDivide => ASTCompoundOp.Divide,
            Keywords.Modulate or
            Keywords.CompoundModulate => ASTCompoundOp.Modulo,
            Keywords.Concatenate or
            Keywords.CompoundConcatenate => ASTCompoundOp.Concatenation,
            _ => throw new InvalidProgramException()
        };
        int idxSplit;
        switch (mat)
        {
            case Keywords.Add:
                if (idxOp != 0) return false;
                idxSplit = tokens.NextTopLevelIndexOf(Keywords.To);
                if (idxSplit == -1) return false;
                goto getASParts;
            case Keywords.Subtract:
                if (idxOp != 0) return false;
                idxSplit = tokens.NextTopLevelIndexOf(Keywords.From);
                if (idxSplit == -1) return false;
                getASParts:

                if (!TryParseExpression(tokens[1..idxSplit], out var ASVal))
                    return false;
                if (!TryParseExpression(tokens[(idxSplit + 1)..], out var ASItm))
                    return false;

                output = new ASTCompoundAssign(tokens.GetASTPosition(),
                    ASItm, op, ASVal);

                return true;
            case Keywords.Multiply:
            case Keywords.Divide:
                if (idxOp != 0) return false;
                idxSplit = tokens.NextTopLevelIndexOf(Keywords.By);
                if (idxSplit == -1) return false;
                goto getMDParts;
            case Keywords.Modulate:
            case Keywords.Concatenate:
                if (idxOp != 0) return false;
                idxSplit = tokens.NextTopLevelIndexOf(Keywords.With);
                if (idxSplit == -1) return false;
                getMDParts:

                if (!TryParseExpression(tokens[1..idxSplit], out var MDObj)) return false;
                if (!TryParseExpression(tokens[(idxSplit + 1)..], out var MDVal)) return false;

                output = new ASTCompoundAssign(tokens.GetASTPosition(),
                    MDObj, op, MDVal);

                return true;
            case Keywords.CompoundAdd:
            case Keywords.CompoundSubtract:
            case Keywords.CompoundMultiply:
            case Keywords.CompoundDivide:
            case Keywords.CompoundModulate:
            case Keywords.CompoundConcatenate:
                if (idxOp < 1 || idxOp >= tokens.Length - 1) return false;

                if (!TryParseExpression(tokens[..idxOp], out var obj)) return false;
                if (!TryParseExpression(tokens[(idxOp + 1)..], out var val)) return false;

                output = new ASTCompoundAssign(tokens.GetASTPosition(), obj, op, val);
                return true;
        }
        return false;
    }

    private static bool TryParseContinue(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].IsNotKeyword(Keywords.Continue)) { return false; }
        output = new ASTContinue(tokens.GetASTPosition());
        return true;
    }

    private static bool TryParseBreak(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 1 || tokens[0].IsNotKeyword(Keywords.Break)) { return false; }
        output = new ASTBreak(tokens.GetASTPosition());
        return true;
    }
    /// <summary>
    /// Attempt to parse a break statement.
    /// ('break')
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseBlock(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 2
            || tokens[0].IsKeyword(Keywords.BlockStart))
        {
            return false;
        }
        var innerTokens = tokens[1..];
        if (tokens[^1].IsKeyword(Keywords.BlockEnd))
            innerTokens = tokens[..^1];

        var nodes = new List<ASTNode>();
        if (!TryParseArray(nodes, innerTokens))
        {
            Error(nameof(TryParseBlock), "Error parsing block contents.", tokens);
            return false;
        }
        output = new ASTBlock(tokens.GetASTPosition(), [.. nodes]);
        return true;
    }
    /// <summary>
    /// Attempt to parse an if statement.
    /// ('if' EXPRESSION 'then' STATEMENT)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseIf(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length < 4) return false;
        if (tokens[0].IsNotKeyword(Keywords.If)) return false; // Fast fails no error
        int idxThen = tokens.NextTopLevelIndexOf(Keywords.Then);
        if (idxThen == -1)
        {
            // Its definitely an if and theres definity no 'then'
            Error(nameof(TryParseIf), "Missing then from if statement.", tokens);
            return false;
        }
        if (!TryParseExpression(tokens[1..idxThen], out var condition))
        {
            // Definitly an if expression parse failure
            Error(nameof(TryParseIf), "Failed to parse if conditional expression.", tokens);
            return false;
        }
        if (!TryParse(tokens[(idxThen + 1)..], out var body) ||
            body is not IStandalone)
        {
            // Definitely an if body parse failure.
            Error(nameof(TryParseIf), "Failed to parse if body statement.", tokens);
            return false;
        }
        output = new ASTIf(tokens.GetASTPosition(), condition, body);
        return true;
    }
    /// <summary>
    /// Attempt to parse an assignment.
    /// ('put' EXPRESSION 'into' EXPRESSION) | ('set'|'assign' EXPRESSION 'equal to'|',' EXPRESSION)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Attempt to parse a variable definition.
    /// ('define'? IDENTIFIER 'is a' ['constant'|'static']? TYPE ['equal to' EXPRESSION]?)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseVarDef(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        int idx_ofIsA = tokens.NextTopLevelIndexOf(Keywords.IsA);
        if ((idx_ofIsA != 1 && idx_ofIsA != 2)) { return false; }
        if (idx_ofIsA == 2 && tokens[0].IsNotKeyword(Keywords.Define)) { return false; }
        if (!TryParseIdentifier([tokens[(idx_ofIsA == 2 ? 1 : 0)]], out var tempId) || tempId is not ASTIdentifier identifier)
        {
            Error(nameof(TryParseVarDef), "Error failed to parse definition identifier.", tokens);
            return false;
        }
        ASTVariableType varType = tokens[(idx_ofIsA == 2 ? 3 : 2)].Keyword switch
        {
            Keywords.Constant => ASTVariableType.Constant,
            Keywords.Static => ASTVariableType.Static,
            _ => ASTVariableType.Generic
        };
        int idx_ofAssign = tokens.NextTopLevelIndexOf(Keywords.EqualTo);
        ASTExpression? initValue = null;
        if (idx_ofAssign != -1 && !TryParseExpression(tokens[(idx_ofAssign + 1)..], out initValue))
        {
            Error(nameof(TryParseVarDef), "Error failed to parse definition initial value.", tokens);
            return false;
        }
        // Get type info
        int srt = (idx_ofIsA == 2 ? (varType != ASTVariableType.Generic ? 4 : 3) :
            (varType != ASTVariableType.Generic ? 3 : 2));
        if (!TryParseTypeInfo((idx_ofAssign == -1 ? tokens[srt..] : tokens[srt..idx_ofAssign]), out var typeInfo))
        {
            Error(nameof(TryParseVarDef), "Error failed to parse definition type.", tokens);
            return false;
        }
        output = new ASTVariableDefinition(tokens.GetASTPosition(), identifier, varType, typeInfo, initValue);
        return true;
    }
    /// <summary>
    /// Attempt to parse a goto statement.
    /// ('goto' IDENTIFIER)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseGoto(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 2
            || tokens[0].Keyword != Keywords.Goto
            || tokens[1].Type != TokenType.Identifier)
            return false;
        if (!TryParseIdentifier([tokens[1]], out var tempId) || tempId is not ASTIdentifier identifier)
        {
            Error(nameof(TryParseGoto), "Error parsing goto statement identifier.", tokens);
            return false;
        }
        output = new ASTGoto(tokens.GetASTPosition(), identifier);
        return true;
    }
    /// <summary>
    /// Attempt to parse a label statement.
    /// ('label' IDENTIFIER)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private bool TryParseLabel(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length != 2
            || tokens[0].Keyword != Keywords.Label
            || tokens[1].Type != TokenType.Identifier)
            return false;
        if (!TryParseIdentifier([tokens[1]], out var tempId) || tempId is not ASTIdentifier identifier)
        {
            Error(nameof(TryParseLabel), "Error parsing label statement identifier.", tokens);
            return false;
        }
        output = new ASTLabel(tokens.GetASTPosition(), identifier); // (ASTIdentifier)nId!);
        return true;
    }
    /// <summary>
    /// Attempt to parse the carmen entry point.
    /// (program BLOCK)
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    public bool TryParseEPoint(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
    {
        output = null;
        if (tokens.Length <= 2 || tokens[0].IsNotKeyword(Keywords.Program))
            return false;

        if (!TryParse(tokens[1..], out var code))
        {
            Error(nameof(TryParseEPoint), "Error parsing entry point statement.", tokens);
            return false;
        }

        output = new ASTEntryPoint(tokens.GetASTPosition(), code);
        return true;
    }
}
