using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.AST.Statements;
using Arcane.Carmen.Collector.Lexer;
using Arcane.Carmen.Collector.Lexer.Tokens;
using Arcane.Carmen.Collector.Validators;
using System.Text;

namespace Arcane.Carmen.Collector.Transcriber
{
    /// <summary>
    /// Outputs the given ast as a .cs file
    /// </summary>
    public class TranscriberCS : IVisitor
    {
        private List<string> bufferLines = [];
        private StringBuilder lineBuffer = new();

        public void Transcribe(ILexer lexer, IValidator validator, string fileIn, string fileOut, bool overwrite) 
        {
            lineBuffer.Clear();
            bufferLines.Clear();
            if (!File.Exists(fileIn)) throw new FileNotFoundException($"File : {fileIn} missing.");
            if (File.Exists(fileOut) && !overwrite) throw new IOException($"File: {fileOut} already exists.");
            string fileContent = File.ReadAllText(fileIn);
            var tokens = lexer.Lex(fileContent);
            tokens.Prepend(new(TokenType.BlockStart, "", 0, 0)); // Add a block start to ensure parser starts as a full block.
            if (!Statement.TryParse(tokens, out var statement) || statement is null) throw new InvalidDataException("Could not parse file contents.");
            statement.Accept(this);
            File.WriteAllLines(fileOut, bufferLines);
        }

        private void PushLine()
        {
            bufferLines.Add(lineBuffer.ToString());
            lineBuffer.Clear();
        }

        public void Visit(IVisitable visitable)
        {
            switch (visitable)
            {
                case Expression expression:
                    Visit(expression);
                    break;
                case Statement statement:
                    Visit(statement);
                    break;
                default:
                    throw new NotImplementedException("Unrecognized AST symbol.");
            }
        }

        private void Visit(Expression expression)
        {
            switch (expression)
            {
                case ExprIdentifier identifier: VisitIdentifierExpression(identifier); break;
                case ExprFunctionCall functionCall: VisitFunctionCallExpression(functionCall); break;
                default:
                    throw new NotSupportedException($"Unsuported expression {expression}.");
            }
        }

        private void Visit(Statement statement)
        {
            switch (statement)
            {
                case StmtFunctionCall functionCall: VisitFunctionCallStatement(functionCall); break;
                case StmtContinue: VisitContinue(); break;
                case StmtBreak: VisitBreak(); break;
                case StmtGoto stmtGoto: VisitGoTo(stmtGoto); break;
                case StmtLabel stmtLabel: VisitLabel(stmtLabel); break;
                default:
                    throw new NotSupportedException($"Unsuported statement {statement}.");
            }
        }

        private void VisitIdentifierExpression(ExprIdentifier identifierExpression)
        {
            switch (identifierExpression.Type)
            {
                case IdentifierType.Structure:
                    lineBuffer.Append("str" + identifierExpression.Name.TrimStart('#')); break;
                case IdentifierType.Label:
                    lineBuffer.Append("lbl" + identifierExpression.Name.TrimStart(':')); break;
                case IdentifierType.Variable:
                    lineBuffer.Append("var" + identifierExpression.Name.TrimStart('$')); break;
                case IdentifierType.Alias:
                    lineBuffer.Append("ali" + identifierExpression.Name.TrimStart('_')); break;
                case IdentifierType.Function:
                    lineBuffer.Append("fnc" + identifierExpression.Name.TrimStart('@')); break;
                case IdentifierType.Type:
                    lineBuffer.Append(identifierExpression.Name); break;
                default:
                    throw new NotSupportedException($"Unsuported Identifier Token {identifierExpression.Type}");
            }
        }

        private void VisitFunctionCallExpression(ExprFunctionCall functionCall)
        {
            functionCall.Identifier.Accept(this);
            lineBuffer.Append('(');
            if (functionCall.Argument != null)
            {
                functionCall.Argument.Accept(this);
            }
            lineBuffer.Append(");");
        }

        private void VisitFunctionCallStatement(StmtFunctionCall functionCall)
        {
            functionCall.FunctionCall.Accept(this);
        }

        private void VisitContinue()
        {
            lineBuffer.Append("continue;");
            PushLine();
        }

        private void VisitBreak()
        {
            lineBuffer.Append("break;");
            PushLine();
        }

        private void VisitGoTo(StmtGoto stmtGoto)
        {
            lineBuffer.Append("goto ");
            stmtGoto.Identifier.Accept(this);
            lineBuffer.Append(";");
            PushLine();
        }

        private void VisitLabel(StmtLabel stmtLabel)
        {
            stmtLabel.Identifier.Accept(this);
            lineBuffer.Append(':');
            PushLine();
        }
    }
}
