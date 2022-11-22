﻿using Translator.Core.Lexing;

namespace Translator.Core.Execution.Operations.Binary
{
    public class SubtractionOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Subtraction";

        protected override TokenTypes Operator => TokenTypes.Minus;
    }
}