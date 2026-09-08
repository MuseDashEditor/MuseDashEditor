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
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace MuseDashEditor.Game.Component.Common;

public partial class BasicScrollContainer(Direction scrollDirection = Direction.Vertical) : BasicScrollContainer<Drawable>(scrollDirection);

public partial class BasicScrollContainer<T>(Direction scrollDirection = Direction.Vertical) : ScrollContainer<T>(scrollDirection)
    where T : Drawable
{
    protected override ScrollbarContainer CreateScrollbar(Direction direction) => new BasicScrollbar(direction);

    private partial class BasicScrollbar : ScrollbarContainer
    {
        private const float dim_size = 8;

        public BasicScrollbar(Direction direction)
            : base(direction)
        {
            Child = new Box()
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.White
            };
        }

        public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
        {
            Vector2 size = new Vector2(dim_size)
            {
                [(int)ScrollDirection] = val
            };
            this.ResizeTo(size, duration, easing);
        }
    }
}
