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

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Component;

public partial class TransparentTextBox : BasicTextBox
{
    protected override Color4 SelectionColour => Colour4.White;

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 16;
        TextContainer.Height = 1;
        BackgroundUnfocused = BackgroundFocused = BackgroundCommit = Colour4.Transparent;
        CornerRadius = 0;
    }
}
