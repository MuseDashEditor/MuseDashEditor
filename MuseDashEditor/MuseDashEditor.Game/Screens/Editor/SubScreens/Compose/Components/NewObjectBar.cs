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

using System.Linq;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Input;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;

public sealed partial class NewObjectBar : FillFlowContainer<NewObjectContainer>
{
    internal NewObjectContainer? SelectedContainer;

    public NewObjectBar()
    {
        Direction = FillDirection.Horizontal;
        RelativeSizeAxes = Axes.X;
        Height = 100;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;
        Children =
        [
            new NewObjectContainer(this, ObjectType.Small, InputAction.Select1),
            new NewObjectContainer(this, ObjectType.Medium1, InputAction.Select2),
            new NewObjectContainer(this, ObjectType.Medium2, InputAction.Select3),
            new NewObjectContainer(this, ObjectType.Large1, InputAction.Select4),
            new NewObjectContainer(this, ObjectType.Large2, InputAction.Select5),
            new NewObjectContainer(this, ObjectType.Raider, InputAction.Select6),
            new NewObjectContainer(this, ObjectType.Hammer, InputAction.Select7),
            new NewObjectContainer(this, ObjectType.Gemini, InputAction.Select8),
            new NewObjectContainer(this, ObjectType.Hold, InputAction.Select9),
            new NewObjectContainer(this, ObjectType.Masher, InputAction.Select10),
            new NewObjectContainer(this, ObjectType.Gear, null),
            new NewObjectContainer(this, ObjectType.Ghost, null),
            new NewObjectContainer(this, ObjectType.Heart, null),
            new NewObjectContainer(this, ObjectType.Note, null)
        ];
    }
}

public partial class NewObjectContainer : Container, IKeyBindingHandler<InputAction>
{
    private readonly NewObjectBar newObjectBar;
    private readonly ObjectType objectType;
    private readonly InputAction? inputAction;
    private readonly Box backgroundBox;
    private readonly Sprite icon;
    private bool selected;

    public NewObjectContainer(NewObjectBar newObjectBar, ObjectType objectType, InputAction? inputAction)
    {
        this.newObjectBar = newObjectBar;
        this.objectType = objectType;
        this.inputAction = inputAction;

        Width = 100;
        Height = 100;
        Children =
        [
            backgroundBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = MdeColors.Background4
            },
            icon = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.75f),
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre
            }
        ];
    }

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore textureStore, EditorDataHolder editorDataHolder)
    {
        var objectData = GameObjectUtils.GetGameObjectData(objectType);
        var laneType = objectData?.ValidLaneTypes.LastOrDefault(LaneType.Ground) ?? LaneType.Ground;
        var sceneType = objectData?.ValidSceneTypes.FirstOrDefault(SceneType.SpaceStation) ?? SceneType.SpaceStation; // TODO: get current scene

        icon.Texture = textureStore.GetObjectTexture(objectType, sceneType, laneType);

        editorDataHolder.CurrentScene.BindValueChanged(@event =>
        {
            icon.Texture = textureStore.GetObjectTexture(objectType, @event.NewValue, laneType);
        }, true);
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (selected)
            return true;

        backgroundBox.TransformTo(nameof(Colour), (ColourInfo)MdeColors.Background2, 150);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (selected)
            return;

        backgroundBox.TransformTo(nameof(Colour), (ColourInfo)MdeColors.Background4, 150);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (selected)
            unselect();
        else
            select();

        return true;
    }

    public bool OnPressed(KeyBindingPressEvent<InputAction> e)
    {
        if (e.Action != inputAction)
            return false;

        if (e.Repeat)
            return true;

        if (selected)
            unselect();
        else
            select();

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
    {
    }

    private void select()
    {
        if (selected)
            return;

        newObjectBar.SelectedContainer?.unselect();
        newObjectBar.SelectedContainer = this;

        backgroundBox.TransformTo(nameof(Colour), (ColourInfo)MdeColors.Background1, 150);
        selected = true;
    }

    private void unselect()
    {
        if (!selected)
            return;

        newObjectBar.SelectedContainer = null;
        backgroundBox.TransformTo(nameof(Colour), (ColourInfo)(IsHovered ? MdeColors.Background2 : MdeColors.Background4), 150);

        selected = false;
    }
}
