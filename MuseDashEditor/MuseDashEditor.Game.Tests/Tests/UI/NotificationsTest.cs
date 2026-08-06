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

using MuseDashEditor.Game.Component.Common;
using MuseDashEditor.Game.Component.Notification;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace MuseDashEditor.Game.Tests.Tests.UI;

public partial class NotificationsTest : MuseDashEditorTestScene
{
    [Test]
    public void TestSimpleConfirmationNotification()
    {
        var notificationContainer = new NotificationContainer
        {
            Depth = float.MinValue
        };

        Add(new Container
        {
            RelativeSizeAxes = Axes.Both,
            Children =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.White,
                    Depth = float.MaxValue
                },
                notificationContainer,
                new RoundedButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(200, 50),
                    Text = "Test",
                    Action = () =>
                    {
                        notificationContainer.SetNotification(new SimpleConfirmationNotification
                        (
                            title: "Are you sure?",
                            message: "This action is irreversible !",
                            onNoAction: () => { },
                            onYesAction: () => { }
                        ));
                        notificationContainer.Show();
                    }
                }
            ]
        });
    }
}
