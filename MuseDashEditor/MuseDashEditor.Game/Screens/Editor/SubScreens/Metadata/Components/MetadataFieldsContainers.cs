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

using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata.Components;

public partial class MetadataFieldsContainers : FillFlowContainer<MetadataField>
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0f, 5f);

        Children =
        [
            new MetadataField
            {
                Label = "Name",
                FieldGetter = chartInfo => chartInfo.NameBindable
            },
            new MetadataField
            {
                Label = "Name romanized",
                FieldGetter = chartInfo => chartInfo.NameRomanizedBindable
            },
            new MetadataField
            {
                Label = "Author",
                FieldGetter = chartInfo => chartInfo.AuthorBindable
            },
            new MetadataField
            {
                Label = "Bpm",
                FieldGetter = chartInfo => chartInfo.BpmBindable
            },
            new MetadataField
            {
                Label = "Level designer",
                FieldGetter = chartInfo => chartInfo.LevelDesignerBindable
            },
            new MetadataField
            {
                Label = "Level difficulty",
                FieldGetter = chartInfo => chartInfo.LevelDifficultyBindable
            }
        ];
    }
}
