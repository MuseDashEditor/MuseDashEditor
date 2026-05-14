using System;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Design;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Timing;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Validation;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens;

public partial class EditorSubscreenContainer : Container<EditorSubscreen>
{
    // Editor subscreens
    private readonly ComposeSubscreen composeSubscreen = new();
    private readonly DesignSubscreen designSubscreen = new();
    private readonly MetadataSubscreen metadataSubscreen = new();
    private readonly TimingSubscreen timingSubscreen = new();
    private readonly ValidationSubscreen validationSubscreen = new();
    private EditorSubscreenType currentScreen;

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        RelativeSizeAxes = Axes.Both;

        AddRange([
            metadataSubscreen,
            timingSubscreen,
            composeSubscreen,
            designSubscreen,
            validationSubscreen
        ]);

        timingSubscreen.Hide();
        composeSubscreen.Hide();
        designSubscreen.Hide();
        validationSubscreen.Hide();

        dataHolder.SelectedSubscreen.ValueChanged += SelectedSubscreenOnValueChanged;
    }

    private void SelectedSubscreenOnValueChanged(ValueChangedEvent<EditorSubscreenType> evt)
    {
        var newValue = evt.NewValue;

        if (currentScreen == newValue) return;

        switch (currentScreen)
        {
            case EditorSubscreenType.Metadata:
                metadataSubscreen.Hide();
                break;
            case EditorSubscreenType.Timing:
                timingSubscreen.Hide();
                break;
            case EditorSubscreenType.Compose:
                composeSubscreen.Hide();
                break;
            case EditorSubscreenType.Design:
                designSubscreen.Hide();
                break;
            case EditorSubscreenType.Validation:
                validationSubscreen.Hide();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (newValue)
        {
            case EditorSubscreenType.Metadata:
                metadataSubscreen.Show();
                break;
            case EditorSubscreenType.Timing:
                timingSubscreen.Show();
                break;
            case EditorSubscreenType.Compose:
                composeSubscreen.Show();
                break;
            case EditorSubscreenType.Design:
                designSubscreen.Show();
                break;
            case EditorSubscreenType.Validation:
                validationSubscreen.Show();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        currentScreen = newValue;
    }

    protected override void LoadComplete()
    {
        LoadComponentAsync(composeSubscreen);
        LoadComponentAsync(designSubscreen);
        LoadComponentAsync(metadataSubscreen);
        LoadComponentAsync(timingSubscreen);
        LoadComponentAsync(validationSubscreen);
    }
}
