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
using osu.Framework.Input.Events;

namespace MuseDashEditor.Game.Component.Common.Menu;

public partial class MenuScrollContainer(Direction direction) : BasicScrollContainer(direction)
{
    protected override bool OnScroll(ScrollEvent e)
    {
        if (ScrollDirection == Direction.Horizontal)
            return false;

        return base.OnScroll(e);
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (ScrollDirection == Direction.Horizontal)
            return false;

        return base.OnDragStart(e);
    }
}
