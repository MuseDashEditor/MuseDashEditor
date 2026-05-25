// Copyright 2026 Axel "Azn9" Joly <contact@azn9.dev>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

using System.Collections.Generic;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Utils;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Chart;

// ReSharper disable InconsistentNaming
public record ChartInfoRaw(
    string name,
    string name_romanized,
    string author,
    string bpm,
    string scene,
    string levelDesigner,
    string levelDesigner1,
    string levelDesigner2,
    string levelDesigner3,
    string levelDesigner4,
    string difficulty1,
    string difficulty2,
    string difficulty3,
    string difficulty4,
    string hideBmsMode,
    string hideBmsDifficulty,
    string hideBmsMessage,
    List<string> searchTags
);
// ReSharper restore InconsistentNaming

public class ChartInfo(ChartInfoRaw raw)
{
    // Global chart info
    public readonly Bindable<string> NameBindable = new(raw.name);
    public readonly Bindable<string> NameRomanizedBindable = new(raw.name_romanized);
    public readonly Bindable<string> AuthorBindable = new(raw.author);
    public readonly Bindable<string> BpmBindable = new(raw.bpm);

    // Current difficulty info
    public readonly Bindable<string> LevelDesignerBindable = new(raw.levelDesigner);
    public readonly Bindable<string> LevelDifficultyBindable = new();
    public readonly Bindable<SceneType> DefaultSceneBindable = new(SceneUtils.GetSceneType(raw.scene));

    // TODO
    // public string HideBmsMode { get; set; } = hideBmsMode; // TODO: enum
    // public string HideBmsDifficulty { get; set; } = hideBmsDifficulty;
    // public string HideBmsMessage { get; set; } = hideBmsMessage;
    // public List<string> SearchTags { get; set; } = searchTags;

    public void LoadDataFromMap(int mapId)
    {
        LevelDesignerBindable.Value = mapId switch
        {
            1 => raw.levelDesigner1,
            2 => raw.levelDesigner2,
            3 => raw.levelDesigner3,
            4 => raw.levelDesigner4,
            _ => LevelDesignerBindable.Value
        };

        LevelDifficultyBindable.Value = mapId switch
        {
            1 => raw.difficulty1,
            2 => raw.difficulty2,
            3 => raw.difficulty3,
            4 => raw.difficulty4,
            _ => LevelDifficultyBindable.Value
        };
    }
}
