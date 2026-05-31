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

using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;

public partial class TimingPointsTableColumn(
    LocalisableString? name = null,
    Anchor anchor = Anchor.Centre,
    [CanBeNull] Dimension dimension = null)
    : TableColumn(name, anchor, dimension);
