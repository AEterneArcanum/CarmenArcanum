namespace Arcane.Carmen.AST.Types
{
    /// <summary>
    /// Basic type info directly visible at the variable defininition.
    /// </summary>
    /// <param name="Identifier">Identifier of type.</param>
    /// <param name="Type">Is it is a basic type which one. NotBase if user defined.</param>
    /// <param name="ArraySize">If it is an array how big is it : null not an array.</param>
    public record ASTTypeInfo(string Identifier, Primitives Type, bool Nullable, bool Pointer, ASTArrayInfo? ArraySize);

    public record ASTArrayInfo(ASTExpression[] Dimensions, ASTTypeInfo ContentType);
}
