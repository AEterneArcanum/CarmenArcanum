using Arcane.Carmen.AST;
using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;
using Arcane.Carmen.Lexer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Parser
{
    public partial class CarmenParser
    {
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
                || TryParseWildcard(tokens, out output)
                || TryParseIndex(tokens, out output)
                || TryParseValue(tokens, out output)
                || TryParseMemberAccess(tokens, out output)
                || TryParseMatch(tokens, out output)
                || TryParseComparison(tokens, out output)
                || TryParseTernary(tokens, out output)
                || TryParseCast(tokens, out output)
                || TryParseArraySlice(tokens, out output)
                || TryParseArrayStride(tokens, out output)
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
                || TryParseIncrement(tokens, out outnd)
                || TryParseFunctionCall(tokens, out outnd))
            {
                output = (ASTExpression)outnd;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryParseFunctionCall(CarmenToken[] tokens, [NotNullWhen(true)] out ASTNode? output)
        {
            output = null;
            if (tokens.Length < 2) { return false; }
            int idxWith = tokens.NextTopLevelIndexOf(Keywords.With);
            if (idxWith != -1 && tokens.Length < 4) { return false; }
            if (idxWith != 2) { return false; }
            if (tokens[0].IsNotKeyword(Keywords.Call)) { return false; }
            // call iden with
            if (!TryParseIdentifier([tokens[1]], out var identifier)) { return false; }
            List<ASTFuncParamImpl> Params = [];
            if (idxWith == 2)
            {
                var parts = tokens[3..].Split(Keywords.Comma);
                foreach (var part in parts)
                {
                    if (!TryParseFunctionParamImpl(part, out var impl)) { return false; }
                    Params.Add(impl);
                }
            }
            output = new ASTFunctionCall(tokens.GetASTPosition(),
                (ASTIdentifier)identifier, [.. Params]);
            return true;
        }

        private bool TryParseFunctionParamImpl(CarmenToken[] tokens, [NotNullWhen(true)] out ASTFuncParamImpl? output)
        {
            output = null;
            int idx = 0;

            bool isRestrict = false;
            if (tokens[idx].IsKeyword(Keywords.Restrict))
            {
                isRestrict = true;
                idx++;
            }

            bool isRef = false;
            if (tokens[idx].IsKeyword(Keywords.Ref))
            {
                isRef = true;
                idx++;
            }

            ASTReadWriteMod rwMod = ASTReadWriteMod.ReadWrite;
            if (tokens[idx].IsKeyword(Keywords.In))
            {
                rwMod = ASTReadWriteMod.ReadOnly;
                idx++;
            }
            else if (tokens[idx].IsKeyword(Keywords.Out))
            {
                rwMod = ASTReadWriteMod.WriteOnly;
                idx++;
            }

            if (!TryParseIdentifier(tokens[idx..], out var identifier))
            {
                return false;
            }

            output = new ASTFuncParamImpl(tokens.GetASTPosition(), 
                (ASTIdentifier)identifier, rwMod, isRef, isRestrict);
            return true;
        }

        private bool TryParseFunctionParamDecl(CarmenToken[] tokens, [NotNullWhen(true)] out ASTFuncParamDecl? output)
        {
            output = null;
            if (tokens.Length < 3)
            {
                return false;
            }
            
            int idx = 0;
            bool isRestrict = false;
            if (tokens[0].IsKeyword(Keywords.Restrict))
            {
                isRestrict = true;
                idx++;
            }
            bool isRef = false;
            if (tokens[idx].IsKeyword(Keywords.Ref))
            {
                isRef = true;
                idx++;
            }
            ASTReadWriteMod rwmod = ASTReadWriteMod.ReadWrite;
            if (tokens[idx].IsKeyword(Keywords.In))
            {
                rwmod = ASTReadWriteMod.ReadOnly;
                idx++;
            }
            else if (tokens[idx].IsKeyword(Keywords.Out))
            {
                rwmod = ASTReadWriteMod.WriteOnly;
                idx++;
            }
            if (!TryParseIdentifier([tokens[idx]], out var identifier))
            {
                return false;
            }
            idx++;
            if (tokens[idx].IsNotKeyword(Keywords.As))
            {
                return false;
            }
            int idxAssign = tokens.NextTopLevelIndexOf(Keywords.EqualTo);
            if (idxAssign != -1)
            {
                if (!TryParseTypeInfo(tokens[idx..idxAssign], out var type))
                {
                    return false;
                }
                if (!TryParseExpression(tokens[(idxAssign + 1)..],  out var expression))
                {
                    return false;
                }
                output = new ASTFuncParamDecl(
                    tokens.GetASTPosition(),
                    (ASTIdentifier)identifier,
                    type,
                    expression,
                    rwmod,
                    isRef,
                    isRestrict);
                return true;
            }
            else
            {
                if (!TryParseTypeInfo(tokens[idx..], out var typeinfo))
                {
                    return false;
                }
                output = new ASTFuncParamDecl(tokens.GetASTPosition(),
                    (ASTIdentifier)identifier, (ASTTypeInfo)typeinfo, null,
                    rwmod, isRef, isRestrict);
                return true;
            }
        }

        private bool TryParseWildcard(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output) 
        {
            output = null;
            if (tokens.Length != 1 || tokens[0].Keyword != Keywords.Wildcard)
                return false;
            output = new ASTWildcard(tokens.GetASTPosition());
            return true;
        }

        private bool TryParseIndex(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output) 
        {
            output = null;
            if (tokens.Length != 1 || tokens[0].Keyword != Keywords.Index)
                return false;
            output = new ASTIndex(tokens.GetASTPosition());
            return true;
        }

        private bool TryParseValue(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length != 1 || tokens[0].Keyword != Keywords.Value)
                return false;
            output = new ASTValue(tokens.GetASTPosition());
            return true;
        }
        /// <summary>
        /// Attempt to parse a match expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool TryParseMatch(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            // Minimum 4 technical
            if (tokens.Length < 5) { return false; }
            if (tokens[0].IsNotKeyword(Keywords.Match)) { return false; }
            // Find semicolon
            int idxCol = tokens.NextTopLevelIndexOf(Keywords.Semicolon);
            if (idxCol == -1 || idxCol < 2 || idxCol >= tokens.Length - 2) { return false; }
            // find required otherwise
            int idxEls = tokens.NextTopLevelIndexOf(Keywords.Otherwise);
            if (idxEls == -1 || idxEls <= idxCol || idxEls >= tokens.Length - 1) { return false;}
            // Get object expression
            if (!TryParseExpression(tokens[1..idxCol], out var objEx)) { return false; }
            // Get Default expression
            if (!TryParseExpression(tokens[idxEls..], out var defEx)) { return false; }
            // Container for parsed matches
            var matches = new List<ASTMatch>();

            if (idxCol + 1 != idxEls)
            {
                var listTokens = tokens[(idxCol + 1)..idxEls];

                // Minimum expr 'with'|'equal to' expr (',' ...
                if (listTokens.Length < 3) { return false; }
                // Try split by commas
                var listItems = listTokens.Split(Keywords.Comma);
                foreach (var item in listItems )
                {
                    int idxWith = item.NextTopLevelIndexOf(Keywords.With);
                    if (idxWith == -1 || idxWith < 1 || idxWith >= tokens.Length - 1) 
                        { return false; }

                    if (!TryParseExpression(item[..idxWith], out var IN)) { return false; }
                    if (!TryParseExpression(item[(idxWith + 1)..], out var OUT)) { return false; }

                    matches.Add(new(item.GetASTPosition(), IN, OUT));
                }
            }
            
            output = new ASTMatchExpression(tokens.GetASTPosition(), objEx, [..matches], defEx);
            return true;
        }
        /// <summary>
        /// Attempt to parse a cast expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool TryParseCast(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length < 4) return false;
            bool safe = true;
            if (tokens[0].IsNotKeyword(Keywords.Cast) &&
                tokens[1].IsNotKeyword(Keywords.Cast))
                return false;
            int idxAs = tokens.NextTopLevelIndexOf(Keywords.AsA);
            if (idxAs == -1 || idxAs < 3 || idxAs >= tokens.Length - 1) return false;

            int str = 1;
            if (tokens[1].IsKeyword(Keywords.Cast))
            {
                if (tokens.Length < 5) return false;
                if (tokens[0].IsNotKeyword(Keywords.Safe) &&
                    tokens[1].IsNotKeyword(Keywords.Unsafe))
                    return false;

                safe = tokens[0].IsKeyword(Keywords.Safe);
                str = 2;
            }

            if (!TryParseExpression(tokens[str..idxAs], out var obj))
                return false;

            if (!TryParseTypeInfo(tokens[(idxAs + 1)..], out var typeInfo))
                return false;

            output = new ASTCastType(tokens.GetASTPosition(), obj, typeInfo, safe);
            return true;
        }
        /// <summary>
        /// Attempt to parse a ternary [inline conditional] expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool TryParseTernary(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length < 6 || tokens[0].IsNotKeyword(Keywords.If)) return false;
            int idxThen = tokens.NextTopLevelIndexOf(Keywords.Then);
            if (idxThen == -1) return false;
            int idxOther = tokens.NextTopLevelIndexOf(Keywords.Otherwise);
            if (idxOther == -1) return false;
            if (!TryParseExpression(tokens[1..idxThen], out var oexpr)) return false;
            if (!TryParseExpression(tokens[(idxThen + 1)..idxOther], out var otrue)) return false;
            if (!TryParseExpression(tokens[(idxOther + 1)..], out var ofalse)) return false;
            output = new ASTTernaryOp(tokens.GetASTPosition(), oexpr, otrue, ofalse);
            return true;
        }
        /// <summary>
        /// Attempt to parse an array stride expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool TryParseArrayStride(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length < 4 || tokens[0].IsNotKeyword(Keywords.Every))
                return false;
            int idxEOf = tokens.NextTopLevelIndexOf(Keywords.ElementOf);
            if (idxEOf == -1)
                return false;
            if (!TryParseExpression(tokens[1..idxEOf], out var stride))
                return false;
            if (!TryParseExpression(tokens[(idxEOf + 1)..], out var list))
                return false;
            output = new ASTArrayStride(tokens.GetASTPosition(), stride, list);
            return true;
        }
        /// <summary>
        /// Attempt to parse an array slice [range] expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool TryParseArraySlice(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length < 5 || tokens[0].IsNotKeyword(Keywords.Elements))
                return false;
            int idxOf = tokens.NextTopLevelIndexOf(Keywords.Of);
            if (idxOf == -1) return false;

            int idxFrom = tokens.NextTopLevelIndexOf(Keywords.From);
            int idxUntil = tokens.NextTopLevelIndexOf(Keywords.Until);
            if (idxUntil == -1 && idxFrom == -1) return false;
            ASTExpression? outend = null;
            ASTExpression? outsrt = null;
            if (idxUntil != -1)
            {
                if (!TryParseExpression(tokens[(idxUntil + 1)..idxOf], out outend))
                    return false;
            }
            if (idxFrom != -1)
            {
                if (idxUntil != -1)
                {
                    if (!TryParseExpression(tokens[(idxFrom + 1)..idxUntil], out outsrt))
                        return false;
                }
                else
                {
                    if (!TryParseExpression(tokens[(idxFrom + 1)..idxOf], out outsrt))
                        return false;
                }
            }
            if (!TryParseExpression(tokens[(idxOf + 1)..], out var outex))
            { return false; }
            output = new ASTArraySlice(tokens.GetASTPosition(), outsrt, outend, outex);
            return true;
        }
        /// <summary>
        /// Attempt to parse a member access expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Attempt to parse a concatenation expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Attempt to parse a null coalesce expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Attempt to parse a bitwise operation except not.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        /// <exception cref="InvalidProgramException"></exception>
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
        /// <summary>
        /// Attempt to [arse a bitwise not expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool TryParseBitwiseNot(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            // General fast fail.
            if (tokens.Length < 2 || tokens[0].IsNotKeyword(Keywords.BitwiseNot))
                return false;
            if (!TryParseExpression(tokens, out var innerExpr))
                Error(nameof(TryParseBitwiseNot), "Failed to parse unitary inner expression.", tokens[1..]);
            output = new ASTBitwiseNot(tokens.GetASTPosition(), innerExpr!);
            return true;
        }
        /// <summary>
        /// Attempt to parse a methematic expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        /// <exception cref="InvalidProgramException"></exception>
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
        /// <summary>
        /// Attempt to parse a boolean expression, except not.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        /// <exception cref="InvalidProgramException"></exception>
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
        /// <summary>
        /// Attempt to parse a boolean not expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Attempt to parse an array accessor expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool TryParseArrayAccess(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length < 4 || tokens[0].IsNotKeyword(Keywords.Index)) return false;
            int idx_of = tokens.NextTopLevelIndexOf(Keywords.Of);
            if (idx_of < 2) return false;
            if (!TryParseExpression(tokens[1..idx_of], out var idexpr))
            {
                Error(nameof(TryParseArrayAccess), "Error parsing array access index.", tokens[1..idx_of]);
                return false;
            }
            if (!TryParseExpression(tokens[(idx_of + 1)..], out var obexpr))
            {
                Error(nameof(TryParseArrayAccess), "Error parsing array access object.", tokens[(idx_of + 1)..]);
                return false;
            }
            output = new ASTArrayAccess(tokens.GetASTPosition(), idexpr!, obexpr!);
            return true;
        }
        /// <summary>
        /// Attempt to parse a list expression [Lit. Array].
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool TryParseLitList(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length == 0)
                return false;
            if (tokens[0].IsNotKeyword(Keywords.Semicolon))
                return false;
            var parts = new List<ASTExpression>();
            var pieces = tokens[1..].Split(Keywords.Comma);
            foreach (var part in pieces)
            {
                if (!TryParseExpression(part, out var expr))
                {
                    Error(nameof(TryParseLitList), "Error parsing list element.", part);
                    return false;
                }
                parts.Add(expr!);
            }
            output = new ASTLitList(tokens.GetASTPosition(), [.. parts]);
            return true;
        }
        /// <summary>
        /// Attemp to parse the address of expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool TryParseAddressOf(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length == 0 || tokens[0].IsNotKeyword(Keywords.TheAddressOf))
                return false;
            if (!TryParseExpression(tokens, out var expr))
            {
                Error(nameof(TryParseAddressOf), "Error parsing address of expression.", tokens);
                return false;
            }
            output = new ASTAddressOf(tokens.GetASTPosition(), expr!);
            return true;
        }
        /// <summary>
        /// Attempt to parse comparison expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        /// <exception cref="InvalidProgramException"></exception>
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
        /// <summary>
        /// Attempt to parse a parenthetical expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        /// <exception cref="ParserException"></exception>
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
        /// <summary>
        /// Attempt to parse a literal char.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Attempt to parse a literal null.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static bool TryParseLitNull(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length != 1 || tokens[0].Keyword != Keywords.Null)
                return false;
            output = new ASTLitNull(tokens.GetASTPosition());
            return true;
        }
        /// <summary>
        /// Attempt to parse a literal string.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Attempt to parse a literal numeric.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static bool TryParseLitNumber(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length != 1 || tokens[0].Type != TokenType.Number)
                return false;
            output = new ASTLitNumber(tokens.GetASTPosition(), tokens[0].ConvertToWholeNumber());
            return true;
        }
        /// <summary>
        /// Attempt to parse a literal boolean.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static bool TryParseLitBool(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length != 1 ||
                (tokens[0].Keyword != Keywords.True && tokens[0].Keyword != Keywords.False))
                return false;
            output = new ASTLitBool(tokens.GetASTPosition(), tokens[0].Keyword == Keywords.True);
            return true;
        }
        /// <summary>
        /// Attempt to parse as an identifier.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static bool TryParseIdentifier(CarmenToken[] tokens, [NotNullWhen(true)] out ASTExpression? output)
        {
            output = null;
            if (tokens.Length != 1 ||
                tokens[0].Keyword != Keywords.Unknown ||
                tokens[0].Type != TokenType.Identifier) return false; // Keyword invalid id.
            output = new ASTIdentifier(tokens.GetASTPosition(), tokens[0].Content);
            return true;
        }
        /// <summary>
        /// Helper to parse type info.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
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
                if (idx_ofType == -1) Error(nameof(TryParseTypeInfo), "Error parsing array type info: missing type.", tokens);

                List<ASTExpression> dimensions = [];
                ASTTypeInfo? subType = null;
                if (tokens[1].Keyword == Keywords.WithSize)
                {
                    var dimTok = tokens[2..idx_ofType].Split(Keywords.By);
                    foreach (var dim in dimTok)
                    {
                        if (!TryParseExpression(dim, out var dimex))
                        {
                            Error(nameof(TryParseTypeInfo), "Error parsing array dimmension expression.", dim);
                            return false;
                        }
                        dimensions.Add((ASTExpression)dimex!);
                    }
                    //
                    if (!TryParseTypeInfo(tokens[(idx_ofType + 1)..], out subType))
                    {
                        Error(nameof(TryParseTypeInfo), "Error parsing array sub type expression.", tokens);
                        return false;
                    }
                }
                output = new(tokens[0].Content, ptype, isNullable, isPointer, new([.. dimensions], subType!));
                return true;
            }
        }
        /// <summary>
        /// Attempt to parse an increment expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
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
                {
                    Error(nameof(TryParseIncrement), "Error paring inner increment statement.", tokens);
                    return false;
                }
                output = new ASTIncrement(tokens.GetASTPosition(), expr!, true);
                return true;
            }
            else
            {
                if (!TryParseExpression(tokens[..^1], out var expr))
                {
                    Error(nameof(TryParseIncrement), "Error paring inner increment statement.", tokens);
                    return false;
                }
                output = new ASTIncrement(tokens.GetASTPosition(), expr!, false);
                return true;
            }
        }
        /// <summary>
        /// Attempt to parse a decrement expression.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="output"></param>
        /// <returns></returns>
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
                {
                    Error(nameof(TryParseDecrement), "Error paring inner decrement expression.", tokens);
                    return false;
                }
                output = new ASTDecrement(tokens.GetASTPosition(), expr!, true);
                return true;
            }
            else
            {
                if (!TryParseExpression(tokens[..^1], out var expr))
                {
                    Error(nameof(TryParseDecrement), "Error paring inner decrement expression.", tokens);
                    return false;
                }
                output = new ASTDecrement(tokens.GetASTPosition(), expr!, false);
                return true;
            }
        }
    }
}
