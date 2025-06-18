namespace Arcane.Script.Carmen;
public partial class Interpreter
{
    private bool _executeGoToStatement(GoToStatement statement)
    {
        if (!_labelIndex.ContainsKey(statement.Id.Name))
            throw new InvalidProgramException($"Undefined Label {statement.Id.Name}");
        _current = _labelIndex[statement.Id.Name] + 1;
        return true;
    }

    private bool _executeDisplayStatement(DisplayStatement statement)
    {
        var output = _evaluate(statement.Value);
        Console.WriteLine(output);
        return true;
    }

    private bool _executeVarDefStatement(VariableDefinitionStatement statement)
    {
        if (_variables.ContainsKey(statement.Id.Name)) // Var already defined
            throw new Exception($"Variable {statement.Id.Name} is already defined.");
        // Validate Initial Value
        object initial = statement.InitialValue switch
        {
            IntegerLiteral =>
                (statement.Type == VariableType.INTEGER)
                    ? ((IntegerLiteral)statement.InitialValue).Value
                    : throw new InvalidCastException($"Variable {statement.Id.Name} is not integer type."),
            StringLiteral =>
                (statement.Type == VariableType.STRING)
                    ? ((StringLiteral)statement.InitialValue).Value
                    : throw new InvalidCastException($"Variable {statement.Id.Name} is not string type."),
            BooleanLiteral =>
                (statement.Type == VariableType.BOOL)
                    ? ((BooleanLiteral)statement.InitialValue).Value
                    : throw new InvalidCastException($"Variable {statement.Id.Name} is not a boolean type."),
            _ => throw new InvalidProgramException($"No default value for type {statement.Type}")
        };

        // Register Var
        _variables[statement.Id.Name] = new(statement.Id.Name, statement.Type, initial);
        return true;
    }

    private bool _executeIncrementStatement(IncrementStatement statement)
    {
        if (!_variables.ContainsKey(statement.Id.Name))
            throw new InvalidProgramException($"Variable {statement.Id.Name} is undefined.");
        if (_variables[statement.Id.Name].Type != VariableType.INTEGER)
            throw new InvalidProgramException("Variable {s?.Id.Name} is not numeric type.");
        var val = _variables[statement.Id.Name].Value;
        _variables[statement.Id.Name].Value = (int)val + 1;
        return true;
    }

    private bool _executeIfStatement(IfStatement statement)
    {
        var result = _evaluate(statement.Condition);
        if (result is not bool b)
            throw new InvalidProgramException("If expression must return a boolean result.");
        if (b)
        {
            _execute(statement.Execution);
            return true; // Early Return due to attached execute imcrementing
        }
        return false;
    }

    private bool _executePutIntoStatement(PutIntoStatement statement)
    {
        if (!_variables.ContainsKey(statement.Id.Name))
            throw new InvalidProgramException($"Variable {statement.Id.Name} is undefined.");
        object value = _evaluate(statement.Value);
        // Check Types
        switch (_variables[statement.Id.Name].Type)
        {
            case VariableType.INTEGER:
                if (value is string s) value = int.Parse(s);
                if (value is not int)
                    throw new InvalidCastException($"Value {value} is not integer value");
                break;
            case VariableType.STRING:
                if (value is not string)
                    throw new InvalidCastException($"Value {value} is not string value.");
                break;
            case VariableType.BOOL:
                if (value is not bool)
                    throw new InvalidCastException($"Value {value} is not a boolean value.");
                break;
            default:
                throw new InvalidCastException("Value type not supported");
        }
        // Set Variable Value
        _variables[statement.Id.Name].Value = value;
        return true;
    }

    private bool _execute(Statement statement)
    {
        switch (statement)
        {
            case LabelStatement: return true;
            case GoToStatement gt: return _executeGoToStatement(gt);
            case DisplayStatement display: return _executeDisplayStatement(display);
            case VariableDefinitionStatement d: return _executeVarDefStatement(d);
            case IncrementStatement inc: return _executeIncrementStatement(inc);
            case IfStatement ifs: return _executeIfStatement(ifs);
            case PutIntoStatement put: return _executePutIntoStatement(put);
            default:
                throw new InvalidProgramException($"Statement not supported: {statement.GetType().Name}");
        }
    }
}
