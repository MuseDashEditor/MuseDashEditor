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
using osu.Framework.Graphics.Containers;
using osuTK;

namespace MuseDashEditor.Game.Component.Toast;

public sealed partial class ToastContainer : Container
{
    private readonly Container<Toast> container;

    public ToastContainer()
    {
        RelativeSizeAxes = Axes.Both;

        Child = new BasicScrollContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Anchor = Anchor.TopRight,
            Origin = Anchor.TopRight,
            Padding = new MarginPadding(20),
            Child = container = new FillFlowContainer<Toast>
            {
                Direction = FillDirection.Vertical,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(10)
            }
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
    }

    public void Show(Toast toast)
    {
        toast.ToastContainer = this;
        container.Add(toast);
    }

    public void RemoveToast(Toast toast)
    {
        //TODO animation
        container.Remove(toast, false);
    }
}
