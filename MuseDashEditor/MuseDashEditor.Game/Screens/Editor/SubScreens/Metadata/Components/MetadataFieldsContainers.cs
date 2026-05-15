using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata.Components;

public partial class MetadataFieldsContainers : FillFlowContainer<MetadataField>
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0f, 5f);

        Children =
        [
            new MetadataField
            {
                Label = "Name",
                FieldGetter = chartInfo => chartInfo.NameBindable
            },
            new MetadataField
            {
                Label = "Name romanized",
                FieldGetter = chartInfo => chartInfo.NameRomanizedBindable
            },
            new MetadataField
            {
                Label = "Author",
                FieldGetter = chartInfo => chartInfo.AuthorBindable
            },
            new MetadataField
            {
                Label = "Bpm",
                FieldGetter = chartInfo => chartInfo.BpmBindable
            },
            new MetadataField
            {
                Label = "Level designer",
                FieldGetter = chartInfo => chartInfo.LevelDesignerBindable
            },
            new MetadataField
            {
                Label = "Level difficulty",
                FieldGetter = chartInfo => chartInfo.LevelDifficultyBindable
            }
        ];
    }
}
