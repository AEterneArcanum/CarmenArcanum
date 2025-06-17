public enum VariableType
{
    INTEGER, STRING, BOOL
}

public enum BinaryOperation
{
    EQUALTO, GREATERTHAN, LESSTHAN, NOTEQUALTO, NOTGREATERTHAN, NOTLESSTHAN, AND, OR, ISTYPE,
    ADDITION, SUBTRACTION, MULTIPLICATION, DIVISION, MODULUS
}

public class VarContainer
{
    public string Name { get; init; }
    public VariableType Type { get; init; }
    public object Value { get; set; }
    public VarContainer(string name, VariableType type, object value)
    {
        Name = name; Type = type; Value = value;
    }
}

public class Interpreter
{
    private Statement[] _statements = [];

    private Dictionary<string, int> _labelIndex = [];

    private Dictionary<string, VarContainer> _variables = [];

    private int _current = 0;

    public Interpreter(Statement[] statements)
    {
        if (statements.Length == 0 || statements is null)
            throw new InvalidProgramException("No Code Passed");

        _statements = statements;
        // Find Symbols
        for (int i = 0; i < statements.Length; i++)
        {
            if (statements[i] is LabelStatement)
            {
                var s = (LabelStatement)statements[i];
                if (_labelIndex.ContainsKey(s.Id.Name))
                    throw new InvalidProgramException($"Duplicate Labels: {s.Id.Name}");
                _labelIndex.Add(s.Id.Name, i);
            }
        }
        // Entry Point
        _current = 0;
    }

    public void Execute()
    {
        while (_current >= 0 && _current < _statements.Length)
        {
            Execute(_statements[_current]);
        }
    }

    private void Execute(Statement statement)
    {
        if (statement is LabelStatement)
        {
            // Move to next
            _current++;
            return; // early return
        }
        else if (statement is GoToStatement)
        {
            // Jump to label index
            var gt = (GoToStatement)statement;
            if (!_labelIndex.ContainsKey(gt.Id.Name))
                throw new InvalidProgramException($"Undefined Label {gt.Id.Name}");
            _current = _labelIndex[gt.Id.Name] + 1; // + 1 Skip label processing
            return; // early return
        }
        else if (statement is DisplayStatement)
        {
            // Display Evaluation Content
            var output = Evaluate(((DisplayStatement)statement).Value);
            Console.WriteLine(output);
        }
        else if (statement is VariableDefinitionStatement)
        {
            VariableDefinitionStatement d = (VariableDefinitionStatement)statement;
            if (_variables.ContainsKey(d.Id.Name)) // Var already defined
                throw new Exception($"Variable {d.Id.Name} is already defined.");
            // Register Var
            object initial;
            if (d.InitialValue is IntegerLiteral)
            {
                initial = (d.Type == VariableType.INTEGER) ? ((IntegerLiteral)d.InitialValue).Value
                    : throw new InvalidCastException($"Variable {d.Id.Name} is not integer type.");
            }
            else if (d.InitialValue is StringLiteral)
            {
                initial = (d.Type == VariableType.STRING) ? ((StringLiteral)d.InitialValue).Value
                    : throw new InvalidCastException($"Variable {d.Id.Name} is not string type.");
            }
            else if (d.InitialValue is BooleanLiteral)
            {
                initial = (d.Type == VariableType.BOOL) ? ((BooleanLiteral)d.InitialValue).Value
                    : throw new InvalidCastException($"Variable {d.Id.Name} is not a boolean type.");
            }
            else
            {
                throw new InvalidProgramException($"No default value for type {d.Type}");
            }

            _variables[d.Id.Name] = new(d.Id.Name, d.Type, initial);
        }
        else if (statement is IncrementStatement)
        {
            IncrementStatement? s = statement as IncrementStatement;
            if (s is null)
                throw new InvalidProgramException("Increment Statement Null");
            if (!_variables.ContainsKey(s.Id.Name))
                throw new InvalidProgramException($"Variable {s?.Id.Name} is undefined.");
            if (_variables[s.Id.Name].Type != VariableType.INTEGER)
                throw new InvalidProgramException("Variable {s?.Id.Name} is not numeric type.");
            var val = _variables[s.Id.Name].Value;
            _variables[s.Id.Name].Value = (int)val + 1;
        }
        else if (statement is IfStatement a)
        {
            var result = Evaluate(a.Condition);
            if (result is not bool b)
                throw new InvalidProgramException("If expression must return a boolean result.");
            if (b)
            {
                Execute(a.Execution);
                return; // Early Return due to attached execute imcrementing
            }
        }
        else if (statement is PutIntoStatement)
        {
            PutIntoStatement p = (PutIntoStatement)statement;
            if (!_variables.ContainsKey(p.Id.Name))
                throw new InvalidProgramException($"Variable {p.Id.Name} is undefined.");
            object value = Evaluate(p.Value);
            // Check Types
            switch (_variables[p.Id.Name].Type)
            {
                case VariableType.INTEGER:
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
            _variables[p.Id.Name].Value = value;
        }
        else
        {
            throw new InvalidProgramException($"Statement not supported: {statement.GetType().Name}");
        }
        _current++; // fallthrough move forward
    }

    private object Evaluate(Expression expression)
    {
        if (expression is StringLiteral)
        {
            return ((StringLiteral)expression).Value;
        }
        else if (expression is IntegerLiteral)
        {
            return ((IntegerLiteral)expression).Value;
        }
        else if (expression is BooleanLiteral)
            return ((BooleanLiteral)expression).Value;
        else if (expression is Identifier)
        {
            // Is A Variable Reference
            Identifier i = (Identifier)expression;
            if (!_variables.ContainsKey(i.Name))
                throw new InvalidProgramException($"Variable {i.Name} not defined!");
            return _variables[i.Name].Value;
        }
        else if (expression is BinaryExpression)
        {
            BinaryExpression exp = (BinaryExpression)expression;
            var pa = Evaluate(exp.Left);
            var pb = Evaluate(exp.Right);
            switch (exp.Operation)
            {
                case BinaryOperation.OR:
                    // Make sure both are boolean values
                    if (pa is not bool || pb is not bool)
                        throw new InvalidOperationException("Or must be between two boolean values {pa} :: {pb}");
                    if ((bool)pa) return true;
                    else if ((bool)pb) return true;
                    return false;
                case BinaryOperation.AND:
                    // Make sure both are boolean values
                    if (pa is not bool || pb is not bool)
                        throw new InvalidOperationException($"Or must be between two boolean values {pa} :: {pb}");
                    if ((bool)pa && (bool)pb) return true;
                    return false;
                case BinaryOperation.EQUALTO:
                    // Match Types
                    if (pa is string v1 && pb is string v)
                        return v1.Equals(v);
                    if (!(pa.GetType() == pb.GetType()))
                        throw new InvalidOperationException($"Equality Types Must Match {pa} :: {pb}");
                    return pa == pb;
                case BinaryOperation.NOTEQUALTO:
                    // Match Types
                    if (!(pa.GetType() == pb.GetType()))
                        throw new InvalidOperationException($"Equality Types Must Match {pa} :: {pb}");
                    return pa != pb;
                case BinaryOperation.LESSTHAN:
                    // Parts Must be Numeric
                    if (pa is not int || pb is not int)
                        throw new InvalidOperationException($"Comparison Values must be numeric {pa} :: {pb}");
                    return (int)pa < (int)pb;
                case BinaryOperation.NOTLESSTHAN:
                    if (pa is not int || pb is not int)
                        throw new InvalidOperationException($"Comparison Values must be numeric {pa} :: {pb}");
                    return !((int)pa < (int)pb);
                case BinaryOperation.GREATERTHAN:
                    if (pa is not int || pb is not int)
                        throw new InvalidOperationException($"Comparison Values must be numeric {pa} :: {pb}");
                    return (int)pa > (int)pb;
                case BinaryOperation.NOTGREATERTHAN:
                    if (pa is not int || pb is not int)
                        throw new InvalidOperationException($"Comparison Values must be numeric {pa} :: {pb}");
                    return !((int)pa > (int)pb);
                case BinaryOperation.ADDITION:
                    if (pa is not int || pb is not int)
                        throw new InvalidOperationException($"Math Operation values {pa} {pb} must be numeric.");
                        return (int)pa + (int)pb;
                case BinaryOperation.SUBTRACTION:
                    if (pa is not int || pb is not int)
                        throw new InvalidOperationException($"Math Operation values {pa} {pb} must be numeric.");
                        return (int)pa - (int)pb;
                case BinaryOperation.MULTIPLICATION:
                    if (pa is not int || pb is not int)
                        throw new InvalidOperationException($"Math Operation values {pa} {pb} must be numeric.");
                        return (int)pa * (int)pb;
                case BinaryOperation.DIVISION:
                    if (pa is not int || pb is not int)
                        throw new InvalidOperationException($"Math Operation values {pa} {pb} must be numeric.");
                        return (int)pa / (int)pb;
                case BinaryOperation.MODULUS:
                    if (pa is not int || pb is not int)
                        throw new InvalidOperationException($"Math Operation values {pa} {pb} must be numeric.");
                        return (int)pa % (int)pb;
                case BinaryOperation.ISTYPE:
                default:
                    throw new NotImplementedException($"Binary Operation {exp.Operation} is not yet implemented.");
            }
        }
        else
        {
            throw new Exception($"Expression not supported: {expression.GetType().Name}");
        }
    }
}
