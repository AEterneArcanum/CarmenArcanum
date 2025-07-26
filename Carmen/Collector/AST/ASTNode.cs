namespace Arcane.Carmen.Collector.AST
{
    /// <summary>
    /// Math operations.
    /// </summary>
    public enum MathOpCode
    {
        /// <summary>
        /// '+'
        /// </summary>
        Addition,
        /// <summary>
        /// '-'
        /// </summary>
        Subtraction,
        /// <summary>
        /// '*'
        /// </summary>
        Multiplication,
        /// <summary>
        /// '/'
        /// </summary>
        Division,
        /// <summary>
        /// '^'
        /// </summary>
        Power,
        /// <summary>
        /// '%'
        /// </summary>
        Modulus
    }
    /// <summary>
    /// Bitwise operations.
    /// </summary>
    public enum BitOpCode
    {
        /// <summary>
        /// Bitwise not '!'.
        /// </summary>
        Not,
        /// <summary>
        /// Bitwise and.
        /// </summary>
        And,
        /// <summary>
        /// Bitwise or.
        /// </summary>
        Or,
        /// <summary>
        /// Bitwise xor.
        /// </summary>
        Xor,
        /// <summary>
        /// Ratate bits left.
        /// </summary>
        RotateL, 
        /// <summary>
        /// Rotate bits right.
        /// </summary>
        RotateR,
        /// <summary>
        /// Shift bits left.
        /// </summary>
        ShiftL, 
        /// <summary>
        /// Shift bits right.
        /// </summary>
        ShiftR,
    }
    /// <summary>
    /// Operations that return a boolean value.
    /// </summary>
    public enum BooleanOpCode
    {
        /// <summary>
        /// Boolean Not '!'.
        /// </summary>
        Not,                            // '!'
        /// <summary>
        /// Boolean And '&&'.
        /// </summary>
        And,                            // '&&'
        /// <summary>
        /// Boolean Or '||'.
        /// </summary>
        Or,                             // '||'
        /// <summary>
        /// Boolean Xor '^'.
        /// </summary>
        Xor,                            // '^'

        /// <summary>
        /// Comparison Equal '=='.
        /// </summary>
        EqualTo,                        // '=='
        /// <summary>
        /// Comparison Not Equal '!='.
        /// </summary>
        NotEqualTo,                     // '!='
        /// <summary>
        /// Comparison Less Than '<'.
        /// </summary>
        LessThan,                       // '<'
        /// <summary>
        /// Comparison Less Than or Equal '<='.
        /// </summary>
        LessThanOrEqual,                // '<='
        /// <summary>
        /// Comparison Greater Than '>'.
        /// </summary>
        GreaterThan,                    // '>'
        /// <summary>
        /// Comparison Greater Than or Equal '>='.
        /// </summary>
        GreaterThanOrEqual,             // '>='

        /// <summary>
        /// Null check.
        /// </summary>
        IsNull,                         // 'is null'
        /// <summary>
        /// Not Null check.
        /// </summary>
        IsNotNull,                      // 'is not null'

        /// <summary>
        /// Type check.
        /// </summary>
        IsType,                         // 'is 'type''
        /// <summary>
        /// Not Type check.
        /// </summary>
        IsNotType,                      // 'is not 'type''
    }
    /// <summary>
    /// Conteiner for parsed expressions within the ast.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    public record ASTNode(int Line, int Column);
    /// <summary>
    /// Represents expression that return values.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    public record ExValue(int Line, int Column) : ASTNode(Line, Column);

    /// <summary>
    /// Contains a string literal by unenclosed value.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Value">Unenclosed string literal.</param>
    public record ExLitString(int Line, int Column, 
        string Value) : ExValue(Line, Column);
    /// <summary>
    /// Contains a numeric literal as a converted decimal value.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Value">Value of the numeric literal.</param>
    public record ExLitNumber(int Line, int Column, 
        decimal Value) : ExValue(Line, Column);
    /// <summary>
    /// Contains a boolean literal value.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Value">Value of the boolean literal.</param>
    public record ExLitBool(int Line, int Column, 
        bool Value) : ExValue(Line, Column);
    /// <summary>
    /// Represents the null literal in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    public record ExLitNull(int Line, int Column) : ExValue(Line, Column);
    /// <summary>
    /// Represents a character literal in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Value">The character literal.</param>
    public record ExLitChar(int Line, int Column,
        char Value) : ExValue(Line, Column);
    /// <summary>
    /// Represents a base type or user defined identifier (eg. variables, structs, functions, records, classes, and 'bool').
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="IsBase">Is the identifier a base type such as 'struct', 'bool', or 'int'.</param>
    /// <param name="Id">Text value keying this identifier.</param>
    public record ExIdentifier(int Line, int Column, 
        bool IsBase, 
        string Id) : ExValue(Line, Column);
    /// <summary>
    /// Represents a member access reference in the code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Owner">Owner of the accessed member.</param>
    /// <param name="Member">Member to access.</param>
    public record ExMemberAccessor(int Line, int Column, 
        ExIdentifier Owner, 
        ExIdentifier Member) : ExValue(Line, Column);
    /// <summary>
    /// Represents an assert statement in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Condition">Boolean expression or check.</param>
    /// <param name="Message">Message to output on assertion failure.</param>
    public record ExAssert(int Line, int Column, 
        ExConditional Condition, 
        ASTNode Message) : ASTNode(Line, Column);
    /// <summary>
    /// Represents an enclosed expression in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="InnerExpression">Enclosed expression.1</param>
    public record ExParenthised(int Line, int Column, ExValue InnerExpression) 
        : ExValue(Line, Column);
    /// <summary>
    /// Represens a null coalesce in code returning the alt value if the checked is null.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Checked">Item to check.</param>
    /// <param name="Alternate">Alternate value to return.</param>
    public record ExNullCoalesce(int Line, int Column,
        ExValue Checked,
        ExValue Alternate) : ExValue(Line, Column);
    /// <summary>
    /// Expression that resolves to a boolean value.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="OpCode">Operation code.</param>
    /// <param name="Left">Left operand.</param>
    /// <param name="Right">Right operand, null in case of '!'.</param>
    public record ExConditional(int Line, int Column, 
        BooleanOpCode OpCode, 
        ExValue Left, 
        ExValue? Right) : ExValue(Line, Column);
    /// <summary>
    /// Represents bitwise operations in the code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="OpCode">Operation code.</param>
    /// <param name="Left">Left operand.</param>
    /// <param name="Right">Right operand, null in case of '!'.</param>
    public record BitOperation(int Line, int Column,
        BitOpCode OpCode,
        ExValue Left,
        ExValue? Right) : ExValue(Line, Column);
    /// <summary>
    /// Represents a block of code within the AST.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Internals">Internal statements within the block.</param>
    public record ExBlock(int Line, int Column, 
        ASTNode[] Internals) : ASTNode(Line, Column);
    /// <summary>
    /// Represents an assignment statement in the code 'container = value'.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Container">Variable or place to store data.</param>
    /// <param name="Value">Value / data to store.</param>
    public record ExAssign(int Line, int Column, 
        ExIdentifier Container, 
        ASTNode Value) : ASTNode(Line, Column);
    /// <summary>
    /// Represents an increment statement.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="IsPrefix">Is it prefixed.</param>
    /// <param name="Identifier">Container to modify and and return.</param>
    public record ExIncrement(int Line, int Column, 
        bool IsPrefix, 
        ExIdentifier Identifier) : ExValue(Line, Column);
    /// <summary>
    /// Represents a decrement statement.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="IsPrefix">Is it prefixed.</param>
    /// <param name="Identifier">Container to modify and and return.</param>
    public record ExDecrement(int Line, int Column, 
        bool IsPrefix, 
        ExIdentifier Identifier) : ExValue(Line, Column);
    /// <summary>
    /// Gets the address of the identifies variable (or function maybe like c# actions)
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Indentity">Identified variable.</param>
    public record ExAddressOf(int Line, int Column,
        ExValue Indentity) : ExValue(Line, Column);
    /// <summary>
    /// Represents accessing an element of an array collection.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Accessed">The accessed array.</param>
    /// <param name="Index">The index to access.</param>
    public record ExArrayAccess(int Line, int Column,
        ExValue Accessed,
        ExValue Index) : ExValue(Line, Column);
    /// <summary>
    /// Represents an array slice in code returning as an array.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Accessed">The accessed array.</param>
    /// <param name="StartIndex">The start of the slice inclusive.</param>
    /// <param name="EndIndex">The end of the slice exclusive.</param>
    public record ExArraySlice(int Line, int Column,
        ExValue Accessed,
        ExValue StartIndex,
        ExValue EndIndex) : ExValue(Line, Column);
    /// <summary>
    /// Represents direct string concatenation in the code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Left">Left operand.</param>
    /// <param name="Right">Right operand.</param>
    public record ExConcatenate(int Line, int Column,
        ExValue Left,
        ExValue Right) : ExValue(Line, Column);
    /// <summary>
    /// Represent a 'literal' array or list within the code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Internals">Contained values.</param>
    public record ExList(int Line, int Column,
        ExValue[] Internals) : ExValue(Line, Column);
    /// <summary>
    /// Represents a math operation in the code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="OpCode">Math operation.</param>
    /// <param name="Left">Left operand.</param>
    /// <param name="Right">Right operand.</param>
    public record ExMathOp(int Line, int Column,
        MathOpCode OpCode,
        ExValue Left,
        ExValue Right) : ExValue(Line, Column);
    /// <summary>
    /// Representas a ternary statement in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Condition">Condition to check against.</param>
    /// <param name="TrueExpression">Value when true.</param>
    /// <param name="FalseExpression">Value when false.</param>
    public record ExTernary(int Line, int Column,
        ExConditional Condition,
        ExValue TrueExpression,
        ExValue FalseExpression) : ASTNode(Line, Column);
    /// <summary>
    /// Passed parameter rw priveledges.
    /// </summary>
    public enum ParameterReadWrite
    {
        ReadWrite,
        ReadOnly,
        WriteOnly
    }
    /// <summary>
    /// Represents a passed parameter to a function.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Value">Value passed.</param>
    /// <param name="Parity">Pair rw with function definition.</param>
    /// <param name="MarkedRestrict">Parameter is marked as restricted.</param>
    public record ExFuncCallParam(int Line, int Column,
        ExValue Value, 
        ParameterReadWrite Parity, 
        bool MarkedRestrict) : ASTNode(Line, Column);
    /// <summary>
    /// Represents calling a function and getting its return value.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Identifier">Identity of function typically an id or member access.</param>
    /// <param name="Parameters">Passed parameters.</param>
    public record ExFuncCall(int Line, int Column,
        ExValue Identifier,
        ExFuncCallParam[] Parameters) : ExValue(Line, Column);


    #region DEFINITIONS

    public enum VarScope
    {
        Public,
        Private,
        Internal
    }

    public enum VarType
    {
        Ordinary,
        Constant,
        Static,
        Parameter
    }

    /// <summary>
    /// Represents an unumeration within the code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Identifier">Name of the enumeration.</param>
    /// <param name="Enumerations">Name of the enumerated elements.</param>
    public record ExEnumDef(int Line, int Column,
        ExIdentifier Identifier,
        ExIdentifier[] Enumerations) : ASTNode(Line, Column);

    /// <summary>
    /// Defines a variable in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Identifier">Variable ID.</param>
    /// <param name="Type">Variable Type.</param>
    /// <param name="Scope">Variable Scope. Ignored when function local variable.</param>
    /// <param name="Struct">Variable Content Type (eg 'bool', and '#myStruct').</param>
    /// <param name="Default">Variable Default Value || Initialized Value.</param>
    /// <param name="Nullable">Variable nullability.</param>
    /// <param name="Pointer">Is this a raw pointer type.</param>
    public record ExVarDef(int Line, int Column,
        ExIdentifier Identifier,
        VarType Type, VarScope Scope,
        ExIdentifier Struct,
        ExValue? Default,
        bool Nullable,
        bool Pointer) : ASTNode(Line, Column);

    public record ExTypeRestriction(ExIdentifier Alias, ExIdentifier[]? Restrictions);

    /// <summary>
    /// Defines a structure by its members.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Identifier">Structure name.</param>
    /// <param name="Inheritance">Structure inherits from.</param>
    /// <param name="TemplateTypes">Structure template parameters not null when template.</param>
    /// <param name="Scope">Accessablility scope.</param>
    /// <param name="Members">Member variables.</param>
    public record ExStructDef(int Line, int Column,
        ExIdentifier Identifier, ExIdentifier Inheritance, ExTypeRestriction[]? TemplateTypes, VarScope Scope,
        ExVarDef[] Members) : ASTNode(Line, Column);

    /// <summary>
    /// Definition of a function parameter in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Identifier">Parameter name.</param>
    /// <param name="Struct">Parameter type.</param>
    /// <param name="Default">Default value, null if undefined.</param>
    /// <param name="Nullable">Parameter nullability.</param>
    /// <param name="Pointer">Is the parameter a raw pointer.</param>
    /// <param name="Restricted">Is the parameter marker with 'restrict'.</param>
    /// <param name="RWPermission">Function R/W Permission to the variable contents.</param>
    public record ExFuncDefParam(int Line, int Column,
        ExIdentifier Identifier,
        ExIdentifier Struct,
        ExValue? Default,
        bool Nullable,
        bool Pointer,
        bool Restricted,
        ParameterReadWrite RWPermission) : ASTNode(Line, Column);

    /// <summary>
    /// Represents a function definition in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Identifier">Function name.</param>
    /// <param name="Scope">Accessability scope.</param>
    /// <param name="Struct">Associated type of self defining this function as 'member' to another body.</param>
    /// <param name="TemplateRestrictions">Restrictions on template types not null when template.</param>
    /// <param name="Return">Return type</param>
    /// <param name="Parameters">Required parameters.</param>
    /// <param name="Body">Function execution.</param>
    public record ExFuncDef(int Line, int Column,
        ExIdentifier Identifier, VarScope Scope,
        ExIdentifier? Struct,
        ExTypeRestriction[]? TemplateRestrictions,
        ExIdentifier? Return,
        ExFuncDefParam[] Parameters,
        ASTNode Body) : ASTNode(Line, Column);

    #endregion

    #region CONTROL FLOW
    /// <summary>
    /// Represents an else body in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Body">Expressions to execute.</param>
    public record ExElse(int Line, int Column,
        ASTNode Body) : ASTNode(Line, Column);
    /// <summary>
    /// Respresents else if.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Condition">Condition to check against.</param>
    /// <param name="TrueBody">Expression to be executed when true.</param>
    public record ExElif(int Line, int Column,
        ExValue Condition, 
        ASTNode TrueBody) : ASTNode(Line, Column);
    /// <summary>
    /// Represents a conditional if-else chain in the ast.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Condition">Condition to check against.</param>
    /// <param name="TrueBody">Expression to be executed when true.</param>
    /// <param name="Alternates">'else if's Array empty when none present.</param>
    /// <param name="FalseBody">Content of 'else'. Expression to be executed when false. Null when not present.</param>
    public record ExIf(int Line, int Column, 
        ExValue Condition, 
        ASTNode TrueBody, 
        ExElif[] Alternates, 
        ExElse? FalseBody) : ASTNode(Line, Column);

    /// <summary>
    /// Represents a switch expression in code. 
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="CamparedValue">Value to compare</param>
    /// <param name="Body">Null if fallthrough.</param>
    public record ExSwitchCondition(int Line, int Column, ExValue CamparedValue, ASTNode? Body) : ASTNode(Line, Column);
    /// <summary>
    /// Represents a switch statement in code. Switch is an execution control pattern.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Target"></param>
    /// <param name="Paths"></param>
    /// <param name="DefaultPath"></param>
    public record ExSwitch(int Line, int Column,
        ExValue Target, 
        ExSwitchCondition[] Paths,
        ASTNode DefaultPath) : ASTNode(Line, Column);
    /// <summary>
    /// Represents a match pattern in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Pattern">Pattern to try.</param>
    /// <param name="Return">Return if the patterns match.</param>
    public record ExMatchValue(int Line, int Column, ExValue Pattern, ExValue Return) : ASTNode(Line, Column);
    /// <summary>
    /// Represents a match expression in code. Match is a value return expression.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="MatchAgainst">Value to check patterns against.</param>
    /// <param name="Patterns">Patterns to try.</param>
    /// <param name="Default">Default return value.</param>
    public record ExMatchExpression(int Line, int Column,
        ExValue MatchAgainst,
        ExMatchValue[] Patterns,
        ExValue Default) : ASTNode(Line, Column);
    
    /// <summary>
    /// Represents a simple infinite loop.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Body">Expressions to execute.</param>
    public record ExLoop(int Line, int Column, ASTNode Body) : ASTNode(Line, Column);
    /// <summary>
    /// Represents a while statement in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Condition">Condition to check against.</param>
    /// <param name="Body">Expressions to execute.</param>
    public record ExWhile(int Line, int Column, 
        ExConditional Condition, 
        ASTNode Body) : ExLoop(Line, Column, Body);
    /// <summary>
    /// Represents a do while statement.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Condition">Condition to check against.</param>
    /// <param name="Body">Expressions to execute.</param>
    public record ExDoWhile(int Line, int Column, 
        ExConditional Condition,
        ASTNode Body) : ExLoop(Line, Column, Body);
    /// <summary>
    /// Represents a for loop in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Id">Id for local loop variable.</param>
    /// <param name="FromVal">Value for $id to begin with.</param>
    /// <param name="ToVal">Value of $id to stop</param>
    /// <param name="StepVal">Value to automatically step $id with. May be null is not present.</param>
    /// <param name="Body">Expressions to execute.</param>
    public record ExFor(int Line, int Column, 
        ExValue Id,
        ExValue FromVal,
        ExValue ToVal,
        ExValue? StepVal,
        ASTNode Body) : ExLoop(Line, Column, Body);
    /// <summary>
    /// Represents a for each loop in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Identifier">Id to use as reference.</param>
    /// <param name="Collection">Collection to iterate.</param>
    /// <param name="Body">Expressions to execute.</param>
    public record ExForEach(int Line, int Column,
        ExIdentifier Identifier,
        ExValue Collection,
        ASTNode Body) : ExLoop(Line, Column, Body);
    /// <summary>
    /// Represents an iterator loop in code.
    /// Concept: 'iterate [array] { }' <-- provide $index and $item as local variables.
    /// or 'iterate [array] (with value as [itm])? (with index as [idx])? { }'
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Collection">Collection to iterate.</param>
    /// <param name="Body">Expressions to execute.</param>
    public record ExIterator(int Line, int Column,
        ExIdentifier? ValueId,
        ExIdentifier? IndexId,
        ExValue Collection,
        ASTNode Body) : ExLoop(Line, Column, Body);


    /// <summary>
    /// Represents a continue statement in loops.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    public record ExContinue(int Line, int Column) : ASTNode(Line, Column);
    /// <summary>
    /// Represents a break statement in loops.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    public record ExBreak(int Line, int Column) : ASTNode(Line, Column);
    /// <summary>
    /// Represents the return statement in functions.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Value">Value returned.</param>
    public record ExReturn(int Line, int Column, 
        ExValue? Value) : ASTNode(Line, Column);



    /// <summary>
    /// Represents a label in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Identifier">Label identifier.</param>
    public record ExLabel(int Line, int Column, 
        ExIdentifier Identifier) : ASTNode(Line, Column);
    /// <summary>
    /// Represents a goto jump in code.
    /// </summary>
    /// <param name="Line">Original line in code.</param>
    /// <param name="Column">Original column in code.</param>
    /// <param name="Identifier">Label identifier.</param>
    public record ExGoto(int Line, int Column, 
        ExIdentifier Identifier) : ASTNode(Line, Column);



    #endregion
}
