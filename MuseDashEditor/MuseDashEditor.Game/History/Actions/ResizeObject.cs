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

using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object.GameObject;
using osu.Framework.Allocation;

namespace MuseDashEditor.Game.History.Actions;

public partial class ResizeObject(
    GameObject gameObject,
    double oldDuration,
    double newDuration,
    bool start
) : HistoryAction
{
    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    public override void Undo()
    {
        if (start)
        {
            var offset = gameObject.HoldEndObject!.Offset.Value - oldDuration;
            gameObject.Offset.Value = offset;
        }
        else
        {
            var offset = gameObject.Offset.Value + oldDuration;
            gameObject.HoldEndObject!.Offset.Value = offset;
        }

        gameObject.HoldDuration = oldDuration;

        editorDataHolder.OnGameObjectsChanged();
    }

    public override void Redo()
    {
        if (start)
        {
            var offset = gameObject.HoldEndObject!.Offset.Value - newDuration;
            gameObject.Offset.Value = offset;
        }
        else
        {
            var offset = gameObject.Offset.Value + newDuration;
            gameObject.HoldEndObject!.Offset.Value = offset;
        }

        gameObject.HoldDuration = newDuration;

        editorDataHolder.OnGameObjectsChanged();
    }
}
