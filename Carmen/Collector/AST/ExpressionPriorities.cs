using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST
{
    public static class ExpressionPriorities
    {
        public const int Parenthised = 1000;
        public const int FunctionCall = 950;
        public const int MemberAccess = 900;
        public const int ArrayAccess = 850;
        public const int Increment = 800;
        public const int Decrement = 800;
        public const int BitwiseNot = 700;
        public const int BitwiseOperation = 700;
        public const int TypeCast = 650;
        public const int TypeCheck = 650;
        public const int MathOperation = 650;
        public const int Concatenate = 650;
        public const int BitShift = 350;
        public const int BitRotation = 350;
        public const int Comparison = 300;
        public const int NullCheck = 250; // 'is null'
        public const int BooleanNot = 200;
        public const int BooleanOperation = 150;
        public const int NullCoalescing = 50; // '??'
        public const int TernaryExpression = 10;

        public const int Identifier = 0;
        public const int BooleanLiteral = 0;
        public const int CharacterLiteral = 0;
        public const int NullLiteral = 0;
        public const int NumberLiteral = 0;
        public const int StringLiteral = 0;
        public const int ListExpression = 0; // defines the elements in a collection
        public const int ArrayDefinition = 0; // defines an array as a type
        public const int AddressOf = 0; // '&' operator
        public const int FunctionParameter = 0;

        public const int IndexExpression = 850;
        public const int ArraySlice = 850;
        public const int ArrayStride = 850;
    }
}
