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

using System;
using System.IO;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Utils;
using NativeFileDialogNET;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;
using TagLib;

namespace MuseDashEditor.Game.Component;

public partial class FileSelectorField : CompositeComponent
{
    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    public string Label { private get; init; } = null!;

    public FileInfo? selectedFile { get; private set; }
    private SpriteText? fileName;
    private ISystemFileSelector? fileSelector;

    [BackgroundDependencyLoader]
    private void load(GameHost gameHost)
    {
        AutoSizeAxes = Axes.Y;
        Width = 300f;

        InternalChildren =
        [
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                CornerExponent = 2.5f,
                BorderThickness = 2.5f,
                Child = new Box
                {
                    Colour = MdeColors.Dark6,
                    RelativeSizeAxes = Axes.Both,
                }
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(9),
                Spacing = new Vector2(0, 4),

                Children =
                [
                    new SpriteText
                    {
                        Text = Label
                    },
                    fileName = new SpriteText
                    {
                        RelativeSizeAxes = Axes.X,
                        Size = new Vector2(1f, 16),
                        Text = ""
                    }
                ]
            }
        ];
    }

    private void OnFileSelected(FileInfo fileInfo)
    {
        if (fileInfo.Extension != ".mp3" && fileInfo.Extension != ".ogg")
            return;

        try
        {
            var tagLibFile = TagLib.File.Create(fileInfo.FullName);
            var tag = tagLibFile.Tag;

            var chartInfo = editorDataHolder.CurrentChart.Value!.ChartInfo;

            var title = tag.Title;
            if (title != "")
                chartInfo.NameBindable.Value = title;

            var artists = tag.JoinedAlbumArtists;
            if (artists != "")
                chartInfo.AuthorBindable.Value = artists;

            selectedFile = fileInfo;
            fileName!.Text = fileInfo.Name;
        }
        catch (UnsupportedFormatException e)
        {
        }
    }

    protected override bool OnClick(ClickEvent e)
    {
        var fileDialog = new NativeFileDialog()
                         .SelectFile()
                         .AddFilter("Music files", "mp3,ogg");

        var result = fileDialog.Open(out string[]? output, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        if (result == DialogResult.Okay && output is { Length: 1 })
        {
            OnFileSelected(new FileInfo(output[0]));
        }

        fileDialog.Dispose();

        return true;
    }
}
