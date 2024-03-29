﻿using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class RemovingCharacterLeftHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.Backspace;

    public ConsoleModifiers Modifiers => default;

    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        submissionDocument.RemoveLeft();
    }
}