namespace Arcane.Script.Carmen;

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

public partial class Interpreter
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

    /// <summary>
    /// Execute the loaded statements.
    /// </summary>
    public void Execute()
    {
        while (_current >= 0 && _current < _statements.Length)
        {
            if (_execute(_statements[_current])) _current++;
        }
    }
}
