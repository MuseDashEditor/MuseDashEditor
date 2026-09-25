// Copyright 2026 Axel "Azn9" Joly <contact@azn9.dev>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

using System.Collections.Generic;
using osu.Framework.Allocation;

namespace MuseDashEditor.Game.History;

public abstract partial class HistoryAction : IDependencyInjectionCandidate
{
    public abstract void Undo();
    public abstract void Redo();

    public virtual void Inject(IReadOnlyDependencyContainer localDependencies)
    {
        localDependencies.Inject(this);
    }
}

public partial class CompoundHistoryAction(List<HistoryAction> actions) : HistoryAction
{
    public override void Undo()
    {
        foreach (var historyAction in actions)
        {
            historyAction.Undo();
        }
    }

    public override void Redo()
    {
        foreach (var historyAction in actions)
        {
            historyAction.Redo();
        }
    }

    public override void Inject(IReadOnlyDependencyContainer localDependencies)
    {
        foreach (var historyAction in actions)
        {
            historyAction.Inject(localDependencies);
        }
    }
}
