namespace Arcane.Script.Carmen;
public partial class Interpreter
{
    private string _evaluateStringLiteral(StringLiteral stringLiteral)
    {
        return stringLiteral.Value;
    }

    private int _evaluateIntegerLiteral(IntegerLiteral integerLiteral)
    {
        return integerLiteral.Value;
    }

    private bool _evaluateBooleanLiteral(BooleanLiteral booleanLiteral)
    {
        return booleanLiteral.Value;
    }

    private object _evaluateIdentifier(Identifier identifier)
    {
        if (_variables.TryGetValue(identifier.Name, out var value))
            return value.Value;
        throw new InvalidProgramException($"Variable {identifier.Name} not defined!");
    }

    private string _evaluateReceiveInput()
    {
        Console.Write(" > ");
        return Console.ReadLine() ?? "NULL INPUT";
    }

    private object _evaluateBinaryExpression(BinaryExpression binary)
    {
        var ll = _evaluate(binary.Left);
        var lr = _evaluate(binary.Right);
        if (ll is string sl)
        {
            try
            {
                ll = int.Parse(sl);
            }
            catch
            {
                ll = sl.GetHashCode();
            }
        }
        else if (ll is bool bl) ll = bl ? 1 : 0;
        if (lr is string sr)
        {
            try
            {
                lr = int.Parse(sr);
            }
            catch
            {
                lr = sr.GetHashCode();
            }
        }
        else if (lr is bool br) lr = br ? 1 : 0;
        if (ll is int il && lr is int ir)
            switch (binary.Operation)
            {
                case BinaryOperationType.OR: return (il > 0) || (ir > 0);
                case BinaryOperationType.AND: return (il > 0) && (ir > 0);
                case BinaryOperationType.EQUALTO: return il == ir;
                case BinaryOperationType.NOTEQUALTO: return il != ir;
                case BinaryOperationType.LESSTHAN: return il < ir;
                case BinaryOperationType.NOTLESSTHAN: return il >= ir;
                case BinaryOperationType.GREATERTHAN: return il > ir;
                case BinaryOperationType.NOTGREATERTHAN: return il <= ir;
                case BinaryOperationType.ADDITION: return il + ir;
                case BinaryOperationType.SUBTRACTION: return il - ir;
                case BinaryOperationType.MULTIPLICATION: return il * ir;
                case BinaryOperationType.DIVISION: return il / ir;
                case BinaryOperationType.MODULUS: return il % ir;
                case BinaryOperationType.ISTYPE:
                default: throw new NotImplementedException($"Binary Operation {binary.Operation} is not yet implemented.");
            }
        throw new InvalidProgramException("Left and Right Operators Must be convertable to integer values.");
    }

    private object _evaluate(Expression expression)
    {
        switch (expression)
        {
            case StringLiteral stringLiteral: return _evaluateStringLiteral(stringLiteral);
            case IntegerLiteral integerLiteral: return _evaluateIntegerLiteral(integerLiteral);
            case BooleanLiteral booleanLiteral: return _evaluateBooleanLiteral(booleanLiteral);
            case Identifier identifier: return _evaluateIdentifier(identifier);
            case ReceiveInputExpression: return _evaluateReceiveInput();
            case BinaryExpression operation: return _evaluateBinaryExpression(operation);
            default: throw new Exception($"Expression not supported: {expression.GetType().Name}");
        }
    }
}
