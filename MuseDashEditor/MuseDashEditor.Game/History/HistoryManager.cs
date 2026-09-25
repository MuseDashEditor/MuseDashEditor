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
using MuseDashEditor.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace MuseDashEditor.Game.History;

public partial class HistoryManager : Drawable, IKeyBindingHandler<InputAction>
{
    // Maybe add a size limit?
    private readonly Queue<HistoryAction> historyActions = new();
    private readonly Queue<HistoryAction> undoedHistoryActions = new();
    private IReadOnlyDependencyContainer localDependencies = null!;

    private readonly List<HistoryAction> compoundCacheActions = [];
    private bool isCachingForCompound;

    protected override void InjectDependencies(IReadOnlyDependencyContainer dependencies)
    {
        base.InjectDependencies(dependencies);
        localDependencies = dependencies;
    }

    public bool OnPressed(KeyBindingPressEvent<InputAction> e)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (e.Action)
        {
            case InputAction.Undo:
                Undo();
                return true;

            case InputAction.Redo:
                Redo();
                return true;

            default:
                return false;
        }
    }

    public void ClearHistory()
    {
        lock (this)
        {
            historyActions.Clear();
            undoedHistoryActions.Clear();
        }
    }

    public void AddAction(HistoryAction action)
    {
        lock (this)
        {
            if (isCachingForCompound)
            {
                compoundCacheActions.Add(action);
                return;
            }

            undoedHistoryActions.Clear();
            historyActions.Enqueue(action);

            action.Inject(localDependencies);
        }
    }

    public void Undo()
    {
        lock (this)
        {
            doUndo();
        }
    }

    public void Redo()
    {
        lock (this)
        {
            doRedo();
        }
    }

    private void doUndo()
    {
        if (!historyActions.TryDequeue(out var action))
            return;

        action.Undo();
        undoedHistoryActions.Enqueue(action);
    }

    private void doRedo()
    {
        if (!undoedHistoryActions.TryDequeue(out var action))
            return;

        action.Redo();
        historyActions.Enqueue(action);
    }

    public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
    {
    }

    public void StartCompound()
    {
        lock (this)
        {
            isCachingForCompound = true;
        }
    }

    public void EndCompound()
    {
        lock (this)
        {
            var action = new CompoundHistoryAction([.. compoundCacheActions]);
            isCachingForCompound = false;
            compoundCacheActions.Clear();
            AddAction(action);
        }
    }
}
