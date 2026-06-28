// Copyright 2026 Axel "Azn9" Joly <contact@azn9.dev>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MuseDashEditor.Game.Config;
using MuseDashEditor.Resources;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Handlers.Joystick;
using osu.Framework.Input.Handlers.Midi;
using osu.Framework.Input.Handlers.Pen;
using osu.Framework.Input.Handlers.Tablet;
using osu.Framework.Input.Handlers.Touch;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;

namespace MuseDashEditor.Game;

public partial class MuseDashEditorGameBase : osu.Framework.Game
{
    protected override Container<Drawable> Content { get; }

    private MdeConfigManager localConfig = null!;
    private DependencyContainer dependencies = null!;

    protected MuseDashEditorGameBase()
    {
        base.Content.Add(Content = new DrawSizePreservingFillContainer
        {
            TargetDrawSize = new Vector2(1920, 1080),
            Strategy = DrawSizePreservationStrategy.Minimum
        });
    }

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);
        localConfig = new MdeConfigManager(host.Storage);
    }

    [BackgroundDependencyLoader]
    private void load(IRenderer renderer, GameHost gameHost)
    {
        Resources.AddStore(new DllResourceStore(typeof(MuseDashEditorResources).Assembly));
        dependencies.CacheAs(new LargeTextureStore(renderer,
            gameHost.CreateTextureLoaderStore(new ResourceStore<byte[]>([
                new NamespacedResourceStore<byte[]>(Resources, "Textures"),
                new NamespacedResourceStore<byte[]>(Resources, "MuseDashResources/Textures")
            ]))));
        dependencies.CacheAs(new TextureStore(renderer,
            gameHost.CreateTextureLoaderStore(new ResourceStore<byte[]>([
                new NamespacedResourceStore<byte[]>(Resources, "Textures"),
                new NamespacedResourceStore<byte[]>(Resources, "MuseDashResources/Textures")
            ]))));

        dependencies.CacheAs(localConfig);

        disableInputHandlers();
    }

    private void disableInputHandlers()
    {
        var tablet = Host.AvailableInputHandlers.OfType<ITabletHandler>().SingleOrDefault();
        if (tablet != null) tablet.Enabled.Value = false;

        var pen = Host.AvailableInputHandlers.OfType<PenHandler>().SingleOrDefault();
        if (pen != null) pen.Enabled.Value = false;

        var touch = Host.AvailableInputHandlers.OfType<TouchHandler>().SingleOrDefault();
        if (touch != null) touch.Enabled.Value = false;

        var midi = Host.AvailableInputHandlers.OfType<MidiHandler>().SingleOrDefault();
        if (midi != null) midi.Enabled.Value = false;

        var joystick = Host.AvailableInputHandlers.OfType<JoystickHandler>().SingleOrDefault();
        if (joystick != null) joystick.Enabled.Value = false;
    }

    protected override IDictionary<FrameworkSetting, object> GetFrameworkConfigDefaults()
    {
        return new Dictionary<FrameworkSetting, object>
        {
            { FrameworkSetting.WindowMode, WindowMode.Windowed },
            { FrameworkSetting.WindowedSize, new Size(1920, 1080) },
            { FrameworkSetting.WindowedPositionX, 0.5 },
            { FrameworkSetting.WindowedPositionY, 0.5 },
            { FrameworkSetting.FrameSync, FrameSync.Limit8x },
            { FrameworkSetting.ExecutionMode, ExecutionMode.MultiThreaded },
            { FrameworkSetting.ShowUnicode, true },
            { FrameworkSetting.AudioUseExperimentalWasapi, true },
        };
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        return dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    }
}
