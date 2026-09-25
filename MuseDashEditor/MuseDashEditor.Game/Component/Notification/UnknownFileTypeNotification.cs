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
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace MuseDashEditor.Game.Component.Notification;

public partial class UnknownFileTypeNotification : SimpleNotification
{
    public UnknownFileTypeNotification(string fileName, out RoundedButton button)
        : base(
            FontAwesome.Solid.ExclamationTriangle,
            "Unknown file type",
            $"{fileName} cannot be opened",
            [
                button = new RoundedButton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Width = 250,
                    Height = 50,
                    Text = "Close"
                },
            ])
    {
        button.Action = Close;
    }
}
