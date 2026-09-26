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

using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace MuseDashEditor.Game.Component.Toast.Impl;

public partial class SavedToast : Toast
{
    protected override Colour4 ContainerColour => Colour4.FromHex("9be9ba");
    protected override IconUsage? Icon => FontAwesome.Solid.Check;
    protected override LocalisableString Message => "Map saved!";
}
