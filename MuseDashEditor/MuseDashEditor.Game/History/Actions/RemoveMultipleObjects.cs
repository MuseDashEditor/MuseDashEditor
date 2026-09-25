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
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;

namespace MuseDashEditor.Game.History.Actions;

public partial class RemoveMultipleObjects(
    List<GameObject> gameObjects
) : HistoryAction
{
    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    public override void Undo()
    {
        foreach (var gameObject in gameObjects)
        {
            editorDataHolder.AddObject(gameObject);
        }

        editorDataHolder.OnGameObjectsChanged();
    }

    public override void Redo()
    {
        foreach (var gameObject in gameObjects)
        {
            editorDataHolder.RemoveObject(gameObject);
        }

        editorDataHolder.OnGameObjectsChanged();
    }
}
