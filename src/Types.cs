namespace Arcane.Script.Carmen;

/// <summary>
/// Enumerated Keywords And Types
/// </summary>
public enum TokenType
{
    UNKNOWN = -1,

    LABEL, GOTO, IDENTIFIER, DISPLAY, LITERAL_STRING, EOL, LITERAL_INTEGER,
    IS, A, INTEGER, STRING, PUT, INTO,
    // Adding boolean support for conditionals
    BOOL, NOT, EQUAL, LESS, GREATER, THAN, TRUE, FALSE, TO,

    AND, OR, COMMA, THE, OF, SUM, DIFFERENCE, QUOTIENT, PRODUCT, MODULUS,

    INCREMENT, IF, THEN, RECEIVED
}

public enum VariableType
{
    INTEGER, STRING, BOOL
}

public enum BinaryOperationType
{
    EQUALTO, GREATERTHAN, LESSTHAN, NOTEQUALTO, NOTGREATERTHAN, NOTLESSTHAN, AND, OR, ISTYPE,
    ADDITION, SUBTRACTION, MULTIPLICATION, DIVISION, MODULUS
}
