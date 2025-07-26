using Arcane.Carmen.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace Arcane.Carmen.Collector.AST
{
    public class ASTParseException : Exception
    {
        public int Line { get; init; }
        public int Column { get; init; }
        public Token[] Tokens { get; init; }
        public ASTParseException(string message, int line, int column, Token[] tokens) : base(message)
        {
            Line = line;
            Column = column;
            Tokens = tokens;
        }
        public ASTParseException(string message, Token[] tokens) : base(message)
        {
            Tokens = tokens;
            Line = tokens[0].Line;
            Column = tokens[0].Column;
        }
    }

    public static class ASTParser
    {
        /// <summary>
        /// Convert provided tokens to raw unchecked ast nodes
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static List<ASTNode> ParseToken(Token[] tokens)
        {

        }
        /// <summary>
        /// 
        /// Warning: Leaves IfElse Chains as individual statements to be combined later.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private static List<Token[]> SplitTokens(Token[] tokens)
        {
            var result = new List<Token[]>();

            int start = 0;

            do
            {
                int end = GetNextEnd(tokens, start);
                if (end > tokens.Length) end = tokens.Length;
                result.Add(tokens[start..end]);
                start = end + 1;
            } while (start < tokens.Length);

            return result;
        }

        private static int GetNextEnd(Token[] tokens, int start)
        {
            return GetNextEndAfter(tokens, start, start);
        }

        public static int GetNextEndAfter(Token[] tokens, int start, int currentPos)
        {
            if (!tokens.TryGetFirstTopLayerIndexOfFrom([TokenType.EndOfStatement, TokenType.BlockEnd], currentPos, out var nxt))
                return tokens.Length;

            if (tokens[nxt].Type == TokenType.BlockEnd &&
                IsDoWhileLoop(tokens, start, nxt) &&
                tokens.TryGetFirstTopLayerIndexOfFrom(TokenType.EndOfStatement, nxt, out var endAfterDoWhileLoop))
                    return endAfterDoWhileLoop;
            // No Special Situation.
            return nxt;
        }

        private static bool IsDoWhileLoop(Token[] tokens, int start, int blockEnd)
        {
            return tokens[start].Type == TokenType.KeywordDo &&
                blockEnd + 1 < tokens.Length &&
                tokens[blockEnd + 1].Type == TokenType.KeywordWhile;
        }

        private static bool TryParseNode(Token[] tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
            if (tokens.Length == 0) return false;
            // ensure eos and eob is removed
            if (tokens[^1].Type == TokenType.EndOfStatement ||
                tokens[^1].Type == TokenType.BlockEnd)
                tokens = tokens[..^1];
            return false// Is statements are handled manually within parse block

                || TryParseBlock(tokens, out node) 
                || TryParseLoopNode(tokens, out node) 
                || TryParseIterator(tokens, out node)

                || TryParseReturnNode(tokens, out node) 
                || TryParseGotoNode(tokens, out node) 
                || TryParseLabelNode(tokens, out node) 
                || TryParseContinueNode(tokens, out node)
                || TryParseBreakNode(tokens, out node)
                || TryParseIdentifierNode(tokens, out node);
        }
        /// <summary>
        /// 'for [] from [] to [] (step [])?: { }'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool TryParseForLoop(Token[] tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
        }
        /// <summary>
        /// 'for each [] in []: { }'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool TryParseForEach(Token[] tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
        }
        /// <summary>
        /// 'iterate EXPRESSION (with value as [])? (with index as [])?: { }'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool TryParseIterator(Token[] tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
            if (tokens.Length < 4 || tokens[0].Type != TokenType.KeywordIterate)
                return false;
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.PunctuationColon, out var colonIndex))
                throw new ASTParseException("Missing colon in iterate statement", tokens);
            var (valueIdentifier, indexIdentifier, expressionEnd) = ParseIteratorClauses(tokens, colonIndex);
            if (!TryParseCollectionExpression(tokens, expressionEnd, out var collection))
                throw new ASTParseException("Invalid collection expression in iterate statement", tokens[1..expressionEnd]);
            if (!TryParseNode(tokens[(colonIndex + 1)..], out var body))
                throw new ASTParseException("Invalid iterate body", tokens[(colonIndex + 1)..]);

            node = new ExIterator(
                tokens[0].Line,
                tokens[0].Column,
                valueIdentifier,
                indexIdentifier,
                (ExValue)collection,
                body
            );
            return true;
        }
        private static (ExIdentifier? valueId, ExIdentifier? indexId, int expressionEnd) ParseIteratorClauses(Token[] tokens, int colonIndex)
        {
            ExIdentifier? valueId = null;
            ExIdentifier? indexId = null;
            int expressionEnd = colonIndex;

            if (tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordWithValueAs, out var valuePos) && valuePos < colonIndex)
            {
                expressionEnd = valuePos;
                if (!TryParseIdentifierAfterKeyword(tokens, valuePos, colonIndex, out valueId))
                    throw new ASTParseException("Invalid value identifier in iterate statement", tokens[(valuePos + 1)..colonIndex]);
            }

            // Parse 'with index as ID' if present
            if (tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordWithIndexAs, out var indexPos) && indexPos < colonIndex)
            {
                expressionEnd = Math.Min(expressionEnd, indexPos);
                if (!TryParseIdentifierAfterKeyword(tokens, indexPos, colonIndex, out indexId))
                    throw new ASTParseException("Invalid index identifier in iterate statement", tokens[(indexPos + 1)..colonIndex]);
            }

            return (valueId, indexId, expressionEnd);
        }
        private static bool TryParseCollectionExpression(Token[] tokens, int end, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
            if (end <= 1) return false;
            if (!TryParseNode(tokens[1..end], out node) || node is not ExValue)
                return false;
            return true;
        }
        private static bool TryParseIdentifierAfterKeyword(Token[] tokens, int keywordIndex, int limit, [NotNullWhen(true)] out ExIdentifier? id)
        {
            id = null;
            int start = keywordIndex + 1;
            int end = keywordIndex + limit + 1;
            if (start >= limit) return false;
            if (!TryParseNode(tokens[start..end], out var node) || node is not ExIdentifier identifier)
                return false;
            id = identifier;
            return true;
        }
        /// <summary>
        /// 'continue.'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool TryParseContinueNode(Token[] tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
            if (tokens.Length != 1 || tokens[0].Type != TokenType.KeywordContinue) { return false; }
            node = new ExContinue(tokens[0].Line, tokens[0].Column);
            return true;
        }
        /// <summary>
        /// 'break.'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool TryParseBreakNode(Token[] tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
            if (tokens.Length != 1 || tokens[0].Type != TokenType.KeywordBreak) { return false; }
            node = new ExBreak(tokens[0].Line, tokens[0].Column);
            return true;
        }
        /// <summary>
        /// 'return (EXPRESSION)?.'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ASTParseException"></exception>
        private static bool TryParseReturnNode(Token[] tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
            if (tokens.Length == 0 || tokens[0].Type != TokenType.KeywordReturn) return false;
            if (tokens.Length == 1)
            {
                node = new ExReturn(tokens[0].Line, tokens[0].Column, null);
            }
            if (!TryParseNode(tokens[1..], out var retVal) || retVal is not ExValue)
                throw new ASTParseException("Error parsing return value!", tokens[1..]);
            node = new ExReturn(tokens[0].Line, tokens[0].Column, (ExValue)retVal);
            return true;
        }
        /// <summary>
        /// 'loop { }'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ASTParseException"></exception>
        private static bool TryParseLoopNode(Token[] tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
            if (tokens.Length <= 1 || tokens[0].Type != TokenType.Loop) return false;
            if (!TryParseBlock(tokens, out var innerNode)) throw new ASTParseException("Error parsing inner loop:", tokens);
            node = new ExLoop(tokens[0].Line, tokens[0].Column, innerNode);
            return true;
        }

        private static bool IsIfStatement(Token[] tokens)
        {
            return tokens.Length >= 5 && tokens[0].Type == TokenType.KeywordIf;
        }
        /// <summary>
        /// '{ }'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ASTParseException"></exception>
        private static bool TryParseBlock(Token[] tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
            if (tokens.Length < 2 || tokens[0].Type != TokenType.BlockStart) return false;
            // Parse inner nodes
            List<ASTNode> nodes = new List<ASTNode>();
            var inner = tokens[1..];
            var statements = SplitTokens(inner);
            for (int i = 0; i < statements.Count; i++)
            {
                var statement = statements[i];

                if (IsIfStatement(statement))
                {
                    var ifElseChain = CollectIfElseChain(statements, ref i);
                    if (TryParseIfElseChain(ifElseChain, out var ifElseNode))
                    {
                        nodes.Add(ifElseNode);
                    }
                    else
                    {
                        throw new ASTParseException("Unknown IfElseChain Parse Error! in/following statement:", statement);
                    }
                }
                else if (TryParseNode(statement, out var newnode))
                {
                    nodes.Add(newnode);
                }
                else
                {
                    throw new ASTParseException("Unknown Block Parse Error!", tokens);
                }
            }


            node = new ExBlock(tokens[0].Line, tokens[0].Column, [.. nodes]);
            return true;
        }
        /// <summary>
        /// 'if EXPRESSION then: { }'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ASTParseException"></exception>
        private static bool TryParseIfElseChain(List<Token[]> tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
            if (tokens.Count == 0) throw new ASTParseException("Attempt to parse empty is else statement array!", 0, 0, []);
            // elsure first is if rest are elif and only final may be else
            if (tokens.Count >= 1 && !IsIfStatement(tokens[0])) throw new ASTParseException("Invalid if statement passed to parser!", tokens[0]);
            for (int i = 1; i < tokens.Count - 1; i++) {
                if (tokens[i][0].Type != TokenType.KeywordOtherwiseIf)
                {
                    throw new ASTParseException($"Invalid #{i} else if statements passed to parser!", tokens[i]);
                }
            }
            if (tokens[^1][0].Type != TokenType.KeywordOtherwiseIf &&
                tokens[^1][0].Type != TokenType.KeywordOtherwise)
                throw new ASTParseException("Invalid final else if statements passed to parser!", tokens[^1]);
            // parse else ifs and else then parse if with else ifs and else contained
            bool final_is_else = tokens[^1][0].Type == TokenType.KeywordOtherwise;
            int elseifcount = tokens.Count - 1 - (final_is_else? 1 : 0); // -1 for if -1 if final is else
            List<ExElif> elseIfs = [];
            for (int i = 0; i < elseifcount; i++)
            {
                if (!TryParseElifNode(tokens[i + 1], out var lif))
                    throw new ASTParseException($"Error parsing elif #{i} in if block!", tokens[i + 1]);
                elseIfs.Add( lif );
            }
            ExElse? exElse = null;
            if (final_is_else)
            {
                if (!TryParseElseNode(tokens[^1], out exElse))
                    throw new ASTParseException("Error parsing else in if block!", tokens[^1]);
            }
            // try parse initial
            // find then in initial
            if (!tokens[0].TryGetFirstTopLayerIndexOf(TokenType.KeywordThen, out var idxThen))
                throw new ASTParseException("Missing then in if block!", tokens[0]);
            if (tokens[0][idxThen + 1].Type != TokenType.PunctuationColon)
                throw new ASTParseException("Missing colon in if block!", tokens[0]);
            // parse primary condition
            if (!TryParseNode(tokens[0][1..idxThen], out var cond) || cond is not ExValue)
                throw new ASTParseException("Error parsing if block condition!", tokens[0][1..idxThen]);
            // parse primary body
            if (!TryParseNode(tokens[0][(idxThen + 1)..], out var body))
                throw new ASTParseException("Error parsing if block body!", tokens[0][(idxThen + 1)..]);
            node = new ExIf(tokens[0][0].Line, tokens[0][0].Column,
                (ExValue)cond, body, [..elseIfs], exElse);
            return true;
        }
        /// <summary>
        /// 'otherwise: { }'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="exElse"></param>
        /// <returns></returns>
        /// <exception cref="ASTParseException"></exception>
        private static bool TryParseElseNode(Token[] tokens, [NotNullWhen(true)] out ExElse? exElse)
        {
            exElse = null;
            if (tokens.Length < 3 || tokens[0].Type != TokenType.KeywordOtherwise) { return false; }
            if (tokens[1].Type != TokenType.PunctuationColon)
                throw new ASTParseException("Missing colon in else statement!", tokens);
            // try parse body
            if (!TryParseNode(tokens[2..], out var body))
                throw new ASTParseException("Error parsing else body!", tokens[2..]);
            exElse = new ExElse(tokens[0].Line, tokens[0].Column, body);
            return true;
        }
        /// <summary>
        /// 'otherwise if EXPRESSION then: { }'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="elif"></param>
        /// <returns></returns>
        /// <exception cref="ASTParseException"></exception>
        private static bool TryParseElifNode(Token[] tokens, [NotNullWhen(true)] out ExElif? elif)
        {
            elif = null;
            if (tokens.Length < 5 || tokens[0].Type != TokenType.KeywordOtherwiseIf) return false;
            // Find then
            if (!tokens.TryGetFirstTopLayerIndexOf(TokenType.KeywordThen, out var idxThen))
                throw new ASTParseException("Missing when in elif statement!", tokens);
            if (tokens[idxThen + 1].Type != TokenType.PunctuationColon)
                throw new ASTParseException("Missing colon in elif statement!", tokens);
            // try parse condition
            if (!TryParseNode(tokens[1..idxThen], out var cond) || cond is not ExValue)
                throw new ASTParseException("Error parsing elif condition!", tokens[1..idxThen]);
            // try parse body
            if (!TryParseNode(tokens[(idxThen + 1)..], out var body))
                throw new ASTParseException("Error parsing elif body!", tokens[(idxThen + 1)..]);
            elif = new(tokens[0].Line, tokens[0].Column, (ExValue)cond, body);
            return true;
        }

        private static List<Token[]> CollectIfElseChain(List<Token[]> statements, ref int currentIndex)
        {
            var chain = new List<Token[]>();
            int i = currentIndex;
            while (i < statements.Count)
            {
                var current = statements[i];
                if (i == currentIndex) // 'if' <expression> 'then' ':'
                {
                    chain.Add(current);
                    i++;
                }
                else if (current.Length >= 5 && current[0].Type == TokenType.KeywordOtherwiseIf) // 'otherwise if' <expression> 'then' ':'
                {
                    chain.Add(current);
                    i++;
                }
                else if (current.Length >= 3 && current[0].Type == TokenType.KeywordOtherwise) // 'otherwise' ':'
                {
                    chain.Add(current); 
                    i++;
                    break;
                }
                else
                {
                    break;
                }
            }

            currentIndex = i - 1; 
            return chain;
        }
        /// <summary>
        /// 'goto LABEL.'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ASTParseException"></exception>
        private static bool TryParseGotoNode(Token[] tokens, [NotNullWhen(true)] out ASTNode? node)
        {
            node = null;
            if (tokens.Length != 2 || tokens[0].Type != TokenType.KeywordGoto) return false;
            if (!TryParseIdentifierNode(tokens, out var identifier)) throw new ASTParseException("Goto parse failed:", tokens);
            if (identifier is not ExIdentifier exId) throw new ASTParseException("Invalid goto identifier:", tokens);
            node = new ExGoto(tokens[0].Line, tokens[0].Column, exId);
            return true;
        }
        /// <summary>
        /// 'label LABEL.'
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        /// <exception cref="ASTParseException"></exception>
        private static bool TryParseLabelNode(Token[] tokens, [NotNullWhen(true)] out ASTNode? label)
        {
            label = null;
            if (tokens.Length != 2 || tokens[0].Type != TokenType.KeywordLabel) return false; // With this we know label is intended.
            if (!TryParseIdentifierNode(tokens[1..], out var identifier)) throw new ASTParseException("Label parse failed:", tokens);
            if (identifier is not ExIdentifier exId) throw new ASTParseException("Invalid label identifier:", tokens);
            label = new ExLabel(tokens[0].Line, tokens[0].Column, exId);
            return true;
        }
        /// <summary>
        /// [USER_DEFINED]
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        private static bool TryParseIdentifierNode(Token[] tokens, [NotNullWhen(true)] out ASTNode? identifier)
        {
            identifier = null;
            if (tokens.Length != 1 || !tokens[0].Type.IsIdentifier() && !tokens[0].Type.IsType()) return false;
            identifier = new ExIdentifier(tokens[0].Line, tokens[0].Column, tokens[0].Type.IsBaseType(), tokens[0].Raw);
            return true;
        }
    }
}