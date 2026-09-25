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
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Allocation;

namespace MuseDashEditor.Game.History.Actions;

public partial class MoveObject(
    GameObject gameObject,
    LaneType oldLaneType,
    LaneType newLaneType,
    double oldOffset,
    double newOffset
) : HistoryAction
{
    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    public override void Undo()
    {
        var laneObject = gameObject.LaneObject;
        if (laneObject is null)
            return;

        gameObject.Offset.Value = oldOffset;
        gameObject.LaneType = oldLaneType;
        laneObject.Offset = oldOffset;
        laneObject.LaneType = oldLaneType;

        if (gameObject.HoldEndObject is not null)
            gameObject.HoldEndObject.Offset.Value = oldOffset + gameObject.HoldDuration;

        editorDataHolder.OnGameObjectsChanged();
    }

    public override void Redo()
    {
        var laneObject = gameObject.LaneObject;
        if (laneObject is null)
            return;

        gameObject.Offset.Value = newOffset;
        gameObject.LaneType = newLaneType;
        laneObject.Offset = newOffset;
        laneObject.LaneType = newLaneType;

        if (gameObject.HoldEndObject is not null)
            gameObject.HoldEndObject.Offset.Value = newOffset + gameObject.HoldDuration;

        editorDataHolder.OnGameObjectsChanged();
    }
}
