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
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace MuseDashEditor.Game.Component.Notification;

public partial class NotificationContainer : Container
{
    private Container notificationContainer = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        Children =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0.5f
            },
            notificationContainer = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            }
        ];
    }

    public void SetNotification(Notification notification)
    {
        notificationContainer.Add(notification);
    }

    public override void Show()
    {
        if (notificationContainer.Count == 0)
            return;
        if (notificationContainer.Count == 1 && IsPresent)
            return;

        this.FadeIn(300, Easing.InOutSine);
    }

    public override void Hide()
    {
        if (notificationContainer.Count == 0 || !IsPresent)
            return;

        this.FadeOut(300, Easing.OutExpo).Finally(_ =>
        {
            notificationContainer.Clear();
        });
    }

    protected override bool OnHover(HoverEvent e)
    {
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        return true;
    }

    protected override void OnKeyUp(KeyUpEvent e)
    {
    }
}
