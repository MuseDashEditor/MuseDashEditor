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
using System.Collections.Generic;
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

public sealed partial class NewObjectBar : FillFlowContainer<Container>, IKeyBindingHandler<InputAction>
{
    internal NewObjectContainer? SelectedContainer;

    private readonly List<ObjectType> possibleObjectTypes = [];

    private readonly InputAction[] actions =
    [
        InputAction.Select1, InputAction.Select2, InputAction.Select3, InputAction.Select4, InputAction.Select5,
        InputAction.Select6, InputAction.Select7, InputAction.Select8, InputAction.Select9, InputAction.Select10
    ];

    private int currentPage;
    private bool select1IsPreviousPage;
    private bool select10IsNextPage;

    public NewObjectBar()
    {
        Direction = FillDirection.Horizontal;
        RelativeSizeAxes = Axes.X;
        Height = 100;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        foreach (ObjectType objectType in Enum.GetValuesAsUnderlyingType<ObjectType>())
        {
            var gameObjectData = GameObjectUtils.GetGameObjectData(objectType);

            if (gameObjectData is null)
                continue;

            if (gameObjectData.MovementType != MovementType.None || objectType == ObjectType.HoldBody)
                continue;

            possibleObjectTypes.Add(objectType);
        }

        updatePages();
    }

    public bool OnPressed(KeyBindingPressEvent<InputAction> e)
    {
        if (select1IsPreviousPage && e.Action is InputAction.Select1)
        {
            NavigateToPreviousPage();
            return true;
        }

        if (select10IsNextPage && e.Action is InputAction.Select10)
        {
            NavigateToNextPage();
            return true;
        }

        if (SelectedContainer is null || e.Action is not InputAction.Cancel and not InputAction.Escape)
            return false;

        SelectedContainer.Unselect();
        return true;
    }

    internal void NavigateToNextPage()
    {
        currentPage++;
        updatePages();
    }

    internal void NavigateToPreviousPage()
    {
        currentPage--;
        updatePages();
    }

    private void updatePages()
    {
        SelectedContainer?.Unselect();

        var itemsPerPage = 10;
        select1IsPreviousPage = false;
        select10IsNextPage = false;

        if (currentPage > 0)
        {
            itemsPerPage--;
            select1IsPreviousPage = true;
        }

        var itemsInPreviousPages = currentPage > 0 ? 9 + (currentPage - 1) * 8 : 0;
        var remainingItems = possibleObjectTypes.Count - itemsInPreviousPages;

        if (remainingItems > 9)
        {
            itemsPerPage--;
            select10IsNextPage = true;
        }

        var itemsInCurrentPage = possibleObjectTypes.Slice(
            itemsInPreviousPages,
            Math.Min(itemsPerPage, possibleObjectTypes.Count - itemsInPreviousPages)
        );

        Clear();

        var actionIndex = 0;

        if (select1IsPreviousPage)
        {
            Add(new PreviousPageSwitcher(this));
            actionIndex++;
        }

        foreach (var objectType in itemsInCurrentPage)
        {
            Add(new NewObjectContainer(this, objectType, actions[actionIndex++]));
        }

        if (select10IsNextPage)
            Add(new NextPageSwitcher(this));
    }

    public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
    {
    }
}

internal abstract partial class BaseContainer : Container
{
    protected readonly Box BackgroundBox;
    protected bool Selected;

    protected BaseContainer()
    {
        Width = 100;
        Height = 100;
        Children =
        [
            BackgroundBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = MdeColors.Background4
            },
            GetContent()
        ];
    }

    protected abstract Drawable GetContent();

    protected override bool OnHover(HoverEvent e)
    {
        if (!IsPresent)
            return false;

        if (Selected)
            return true;

        BackgroundBox.TransformTo(nameof(Colour), (ColourInfo)MdeColors.Background2, 150);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (Selected || !IsPresent)
            return;

        BackgroundBox.TransformTo(nameof(Colour), (ColourInfo)MdeColors.Background4, 150);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (!IsPresent)
            return false;

        if (Selected)
            Unselect();
        else
            Select();

        return true;
    }

    protected virtual void Select()
    {
    }

    internal virtual void Unselect()
    {
    }
}

internal sealed partial class PreviousPageSwitcher(NewObjectBar newObjectBar) : BaseContainer
{
    protected override Drawable GetContent() => new SpriteIcon
    {
        RelativeSizeAxes = Axes.Both,
        Size = new Vector2(0.35f),
        Origin = Anchor.Centre,
        Anchor = Anchor.Centre,
        Icon = FontAwesome.Solid.ChevronLeft
    };

    protected override bool OnClick(ClickEvent e)
    {
        if (!IsPresent)
            return false;

        newObjectBar.NavigateToPreviousPage();
        return true;
    }
}

internal sealed partial class NextPageSwitcher(NewObjectBar newObjectBar) : BaseContainer
{
    protected override Drawable GetContent() => new SpriteIcon
    {
        RelativeSizeAxes = Axes.Both,
        Size = new Vector2(0.35f),
        Origin = Anchor.Centre,
        Anchor = Anchor.Centre,
        Icon = FontAwesome.Solid.ChevronRight
    };

    protected override bool OnClick(ClickEvent e)
    {
        if (!IsPresent)
            return false;

        newObjectBar.NavigateToNextPage();
        return true;
    }
}

internal partial class NewObjectContainer(NewObjectBar newObjectBar, ObjectType objectType, InputAction? inputAction) : BaseContainer, IKeyBindingHandler<InputAction>
{
    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    private Sprite icon = null!;

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore textureStore)
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

    public bool OnPressed(KeyBindingPressEvent<InputAction> e)
    {
        if (!IsPresent)
            return false;

        if (e.Action != inputAction)
            return false;

        if (e.Repeat)
            return true;

        if (Selected)
            Unselect();
        else
            Select();

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
    {
    }

    protected override void Select()
    {
        if (Selected)
            return;

        newObjectBar.SelectedContainer?.Unselect();
        newObjectBar.SelectedContainer = this;

        editorDataHolder.PlacementObjectType = objectType;
        editorDataHolder.IsInPlacementMode.Value = true;

        BackgroundBox.TransformTo(nameof(Colour), (ColourInfo)MdeColors.Background1, 150);
        Selected = true;
    }

    internal override void Unselect()
    {
        if (!Selected)
            return;

        newObjectBar.SelectedContainer = null;
        BackgroundBox.TransformTo(nameof(Colour), (ColourInfo)(IsHovered ? MdeColors.Background2 : MdeColors.Background4), 150);

        editorDataHolder.IsInPlacementMode.Value = false;
        Selected = false;
    }

    protected override Drawable GetContent() => icon = new Sprite
    {
        RelativeSizeAxes = Axes.Both,
        Size = new Vector2(0.75f),
        Origin = Anchor.Centre,
        Anchor = Anchor.Centre
    };
}
