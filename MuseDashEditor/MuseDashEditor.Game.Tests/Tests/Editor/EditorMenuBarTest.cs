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

using MuseDashEditor.Game.Component.Common.Menu;
using MuseDashEditor.Game.Component.Common.Menu.Item;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace MuseDashEditor.Game.Tests.Tests.Editor;

public partial class EditorMenuBarTest : MuseDashEditorTestScene
{
    public EditorMenuBarTest()
    {
        Add(new Container
        {
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            RelativeSizeAxes = Axes.X,
            Height = 50,
            Y = 50,
            Child = new MdeMenu(Direction.Horizontal, true)
            {
                RelativeSizeAxes = Axes.Both,
                Items =
                [
                    new TopMenuMenuItem("File")
                    {
                        Items =
                        [
                            new SubMenuMenuItem("Open difficulty")
                            {
                                Items =
                                [
                                    new ActionMenuItem("Easy", () => { }),
                                    new ActionMenuItem("Hard", () => { }),
                                    new ActionMenuItem("Master", () => { }),
                                    new ActionMenuItem("Hidden", () => { })
                                ]
                            },
                            new SeparatorMenuItem(),
                            new ActionMenuItem("Save", () => { }),
                            new ActionMenuItem("Save copy as...", () => { }),
                            new ActionMenuItem("Reset to last save", () => { }, MenuItemType.Destructive),
                            new SeparatorMenuItem(),
                            new ActionMenuItem("Export", () => { }),
                            new ActionMenuItem("Export as BMS", () => { }),
                            new SeparatorMenuItem(),
                            new ActionMenuItem("Quit", () => { }),
                            new ActionMenuItem("Quit without saving", () => { }, MenuItemType.Destructive)
                        ]
                    },
                    new TopMenuMenuItem("Edit"),
                    new TopMenuMenuItem("A"),
                    new TopMenuMenuItem("B"),
                    new TopMenuMenuItem("C"),
                ]
            }
        });
    }
}
