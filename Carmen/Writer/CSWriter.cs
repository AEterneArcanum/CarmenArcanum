using Arcane.Carmen.AST;
using Arcane.Carmen.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Writer;

public class CSWriter : IWriter
{
    private readonly StringBuilder sb;
    private int indentLevel = 0;
    private string GetIndent() => new string(' ', indentLevel);

    private void WriteLine(string text)
    {
        sb.AppendLine(text);
        sb.Append(GetIndent());
    }

    public CSWriter() { sb = new(); }
    /// <summary>
    /// Transcribe an ast node array to a specified file.
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="filename"></param>
    /// <exception cref="Exception"></exception>
    public void Write(ASTNode[] nodes, string filename)
    {
        sb.Clear();
        if (!Validate(nodes))
            throw new Exception("Unsupported nodes detected in code!");
        foreach (ASTNode node in nodes) {
            Write(node);
        }
        File.WriteAllText(filename, sb.ToString());
    }
    /// <summary>
    /// Check an ast node array for unssuported code.
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    public bool Validate(ASTNode[] nodes)
    {
        bool clean = true;
        foreach (var node in nodes) {
            switch (node)
            {
                case ASTAssignment:
                case ASTBlock:
                case ASTDecrement:
                case ASTEntryPoint:
                case ASTGoto:
                case ASTIdentifier:
                case ASTIf:
                case ASTIncrement:
                case ASTLabel:
                case ASTLitBool:
                case ASTLitNumber:
                case ASTParenthized:
                case ASTVariableDefinition:
                case ASTComparison:
                    continue;
                default:
                    Console.WriteLine($"Node {node.GetType()} not supported by {nameof(CSWriter)}");
                    clean = false;
                    continue;
            }
        }
        return clean;
    }

    private void Write(ASTNode node, bool subExpr = false)
    {
        switch (node)
        {
            case ASTComparison compa: Write(compa); break;
            case ASTAssignment assignment: Write(assignment); break;
            case ASTVariableDefinition variableDefinition: Write(variableDefinition); break;
            case ASTParenthized parenthized: Write(parenthized); break;
            case ASTIdentifier identifier: Write(identifier); break;
            case ASTLitBool lb: Write(lb); break;
            case ASTLabel label: Write(label); break;
            case ASTIncrement increment: Write(increment, subExpr); break;
            case ASTIf aSTIf: Write(aSTIf); break;
            case ASTLitNumber lt: Write(lt); break;
            case ASTBlock block: Write(block); break;
            case ASTGoto gootoo: Write(gootoo); break;
            case ASTDecrement decrement: Write(decrement, subExpr); break;
            case ASTEntryPoint ep: Write(ep); break;
            default: throw new NotImplementedException($"Node {node.GetType()} not supported by {nameof(CSWriter)}");
        }
    }

    private void Write(ASTComparison compa)
    {
        Write(compa.Left);
        sb.Append(' ');
        sb.Append(compa.Op switch
        {
            ASTComparisonOp.Equal => "== ",
            ASTComparisonOp.NotEqual => "!= ",
            ASTComparisonOp.LessThan => "< ",
            ASTComparisonOp.GreaterThan => "> ",
            ASTComparisonOp.LessThanOrEqual => "<= ",
            ASTComparisonOp.GreaterThanOrEqual => ">= ",
            _ => ""
        });
        Write(compa.Right);
    }

    private void Write(ASTEntryPoint entry)
    {
        Write(entry.Code, false); // For now just dump in file
    }

    private void Write(ASTBlock block)
    {
        sb.Append('{');
        indentLevel++;
        foreach (var itm in block.InnerNodes)
        {
            Write(itm);
        }
        indentLevel--;
        WriteLine("}");
    }

    private void Write(ASTParenthized parenthized)
    {
        sb.Append('(');
        Write(parenthized.InnerExpr);
        sb.Append(')');
    }

    private string GetTypeString(ASTTypeInfo Type)
    {
        return Type.Type switch { 
            BasicTypes.Byte => "byte",
            BasicTypes.Short => "short",
            _ => Type.Identifier
        };
    }

    private void Write(ASTVariableDefinition variableDefinition)
    {
        sb.Append(GetTypeString(variableDefinition.Type));
        if (variableDefinition.Type.ArraySize >= 0)
        {
            sb.Append('['); // Multidimentional array access and support later "[,]"
            if (variableDefinition.Type.ArraySize > 0)
                sb.Append(variableDefinition.Type.ArraySize.ToString());
            sb.Append(']');
        }
        sb.Append(' ');
        Write(variableDefinition.Identifier);

        // Initial value support later

        WriteLine(";");
    }

    private void Write(ASTLitNumber lt)
    {
        sb.Append(lt.Value.ToString());
    }

    private void Write(ASTLitBool lb)
    {
        sb.Append(lb.Value ? "true" : "false");
    }

    private void Write(ASTLabel label)
    {
        Write(label.Identifier, true);
        WriteLine(":");
    }

    private void Write(ASTIf aSTIf)
    {
        sb.Append("if (");
        Write(aSTIf.Condition, true);
        sb.Append(") ");
        Write(aSTIf.Body, false); // Body should have the correct c# block ar single statement terminator.
    }
    
    private void Write(ASTIdentifier identifier)
    {
        sb.Append(identifier.Identifier);
    }

    private void Write(ASTGoto @goto)
    {
        sb.Append("goto ");
        Write(@goto.Identifier, true);
        WriteLine(";");
    }

    private void Write(ASTIncrement increment, bool subExpr = false)
    {
        if (increment.IsPrefix) sb.Append("++");
        Write(increment.Expression, true);
        if (!increment.IsPrefix) sb.Append("++");
        if (!subExpr) WriteLine(";");
    }

    private void Write(ASTDecrement decrement, bool subExpr = false)
    {
        if (decrement.IsPrefix) sb.Append("--");
        Write(decrement.Expression, true);
        if (!decrement.IsPrefix)sb.Append("--");
        if (!subExpr) WriteLine(";");
    }

    private void Write(ASTAssignment assignment)
    {
        Write(assignment.Object, true);
        sb.Append(" = ");
        Write(assignment.Value, true);
        WriteLine(";");
    }
}
