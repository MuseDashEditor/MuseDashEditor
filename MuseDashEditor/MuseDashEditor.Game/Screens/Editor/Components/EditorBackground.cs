using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class EditorBackground : Sprite
{
    [Resolved] protected LargeTextureStore Textures { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        RelativeSizeAxes = Axes.Both;
        Size = new Vector2(1, 1);
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        Alpha = 0.2f;

        dataHolder.CurrentScene.BindValueChanged(OnSceneChange, true);
    }

    private void OnSceneChange(ValueChangedEvent<SceneType> valueChangedEvent)
    {
        var newScene = valueChangedEvent.NewValue;

        if (newScene == SceneType.Unknown)
            Texture = Textures.Get("HomeScreen/background");
        else
        {
            var textureName = $"Scenes/{(int)newScene}_{newScene.ToString().ToLowerInvariant()}/background";
            Texture = Textures.Get(textureName);
        }
    }
}
