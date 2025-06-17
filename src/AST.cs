public abstract record Expression;
public abstract record Statement;

public record StringLiteral(string Value) : Expression;
public record IntegerLiteral(int Value) : Expression;
public record BooleanLiteral(bool Value) : Expression;
public record Identifier(string Name) : Expression;
public record BinaryExpression(BinaryOperation Operation, Expression Left, Expression Right) : Expression;

public record VariableDefinitionStatement(Identifier Id, VariableType Type, Expression InitialValue) : Statement;
public record LabelStatement(Identifier Id) : Statement;
public record GoToStatement(Identifier Id) : Statement;
public record DisplayStatement(Expression Value) : Statement;
public record PutIntoStatement(Identifier Id, Expression Value) : Statement;

public record IncrementStatement(Identifier Id) : Statement;
public record IfStatement(Expression Condition, Statement Execution) : Statement;
