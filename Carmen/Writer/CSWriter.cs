using Arcane.Carmen.AST;
using Arcane.Carmen.AST.Literals;
using Arcane.Carmen.AST.Types;
using System.Text;

namespace Arcane.Carmen.Writer;

public class CSWriter : Writer
{
    private readonly StringBuilder sb;
    private int indentLevel = 0;
    private string GetIndent() => new string(' ', indentLevel);

    private void WriteLine(string text)
    {
        sb.AppendLine(text);
        sb.Append(GetIndent());
    }

    public CSWriter() : base() { sb = new(); }

    public override void Clear()
    {
        sb.Clear();
    }
    public override void SaveFile(string filename)
    {
        File.WriteAllText($"{filename}.cs", sb.ToString());
    }
    public override bool ValidateNode(ASTNode node)
    {
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
            case ASTLitString:
            case ASTLitChar:
            case ASTParenthized:
            case ASTVariableDefinition:
            case ASTComparison:
                return true;
            default:
                return false;
        }
    }
    public override void WriteNode(ASTNode node, bool subExpr = false)
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
            case ASTLitString ls: Write(ls); break;
            case ASTLitChar lc: Write(lc); break;
            case ASTBlock block: Write(block); break;
            case ASTGoto gootoo: Write(gootoo); break;
            case ASTDecrement decrement: Write(decrement, subExpr); break;
            case ASTEntryPoint ep: Write(ep); break;
            default: LogError($"Node {node.GetType()} not supported by {nameof(CSWriter)}"); break;
        }
    }

    private void Write(ASTLitChar lc)
    {
        sb.Append('\'');
        sb.Append(lc.Value);
        sb.Append('\'');
    }

    private void Write(ASTLitString aSTLitString)
    {
        sb.Append('"'); 
        sb.Append(aSTLitString.Value); 
        sb.Append('"');
    }

    private void Write(ASTComparison compa)
    {
        WriteNode(compa.Left);
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
        WriteNode(compa.Right);
    }

    private void Write(ASTEntryPoint entry)
    {
        WriteLine("namespace Aracane;");
        WriteLine("internal class Program { ");
        indentLevel++;
        sb.Append("static int Main(string[] args)");
        WriteNode(entry.Code, false); // For now just dump in file
        indentLevel--;
        WriteLine("}");
    }

    private void Write(ASTBlock block)
    {
        WriteLine("{");
        indentLevel++;
        foreach (var itm in block.InnerNodes)
        {
            WriteNode(itm);
        }
        indentLevel--;
        WriteLine("}");
    }

    private void Write(ASTParenthized parenthized)
    {
        sb.Append('(');
        WriteNode(parenthized.InnerExpr);
        sb.Append(')');
    }

    private string GetTypeString(ASTTypeInfo Type)
    {
        return Type.Type switch { 
            Primitives.Byte => "byte",
            Primitives.SByte => "signed byte",
            Primitives.Short => "short",
            Primitives.UShort => "unsigned short",
            Primitives.Integer => "int",
            Primitives.UInteger => "unsingned int",
            Primitives.Long => "long",
            Primitives.ULong => "unsigned long",
            Primitives.Float => "float",
            Primitives.Double => "double",
            Primitives.Decimal => "decimal",
            Primitives.Void => "void",
            Primitives.String => "string",
            _ => Type.Identifier
        };
    }

    private void WriteType(ASTTypeInfo typeInfo)
    {
        // Implement after type expansion
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
        WriteNode(label.Identifier, true);
        WriteLine(":;");
    }

    private void Write(ASTIf aSTIf)
    {
        sb.Append("if (");
        WriteNode(aSTIf.Condition, true);
        sb.Append(") ");
        WriteNode(aSTIf.Body, false); // Body should have the correct c# block ar single statement terminator.
    }
    
    private void Write(ASTIdentifier identifier)
    {
        sb.Append(identifier.Identifier);
    }

    private void Write(ASTGoto @goto)
    {
        sb.Append("goto ");
        WriteNode(@goto.Identifier, true);
        WriteLine(";");
    }

    private void Write(ASTIncrement increment, bool subExpr = false)
    {
        if (increment.IsPrefix) sb.Append("++");
        WriteNode(increment.Expression, true);
        if (!increment.IsPrefix) sb.Append("++");
        if (!subExpr) WriteLine(";");
    }

    private void Write(ASTDecrement decrement, bool subExpr = false)
    {
        if (decrement.IsPrefix) sb.Append("--");
        WriteNode(decrement.Expression, true);
        if (!decrement.IsPrefix)sb.Append("--");
        if (!subExpr) WriteLine(";");
    }

    private void Write(ASTAssignment assignment)
    {
        WriteNode(assignment.Object, true);
        sb.Append(" = ");
        WriteNode(assignment.Value, true);
        WriteLine(";");
    }
}
