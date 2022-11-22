﻿using Translator.Core.Execution.Operation.Binary;
using Translator.Core.Lexing;

namespace Translator.Core.Execution.Operations.Binary
{
    public class AdditiveOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Addition";

        protected override TokenTypes Operator => TokenTypes.Plus;
    }
}