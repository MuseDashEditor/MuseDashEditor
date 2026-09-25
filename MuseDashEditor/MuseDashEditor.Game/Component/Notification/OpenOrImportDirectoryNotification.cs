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

using System.IO;
using MuseDashEditor.Game.Component.Common;
using MuseDashEditor.Game.Project;
using osu.Framework.Graphics.Sprites;

namespace MuseDashEditor.Game.Component.Notification;

public partial class OpenOrImportDirectoryNotification : SimpleNotification
{
    public OpenOrImportDirectoryNotification(DirectoryInfo directoryInfo, out RoundedButton closeButton)
        : base(
            FontAwesome.Solid.QuestionCircle,
            "What do you want to do",
            $"Do you want to open the chart from {directoryInfo.Name} or import it as a project?",
            [
                new RoundedButton
                {
                    Width = 250,
                    Height = 50,
                    Text = "Open",
                    Action = () =>
                    {
                        ImportManager.ImportManagerInstance.Open(directoryInfo);
                    }
                },
                new RoundedButton
                {
                    Width = 250,
                    Height = 50,
                    Text = "Import",
                    Action = () =>
                    {
                        ImportManager.ImportManagerInstance.Import(directoryInfo);
                    }
                },
                closeButton = new RoundedButton
                {
                    Width = 250,
                    Height = 50,
                    Text = "Cancel"
                },
            ])
    {
        closeButton.Action = Close;
    }
}
