using System;
using MuseDashEditor.Game.Editor.Clock;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Layout;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class ZoomableScrollContainer : ZoomableScrollContainer<Drawable>
{
    private const float zoom_speed = 0.02f;

    [Resolved] private EditorClock editorClock { get; set; } = null!;

    private readonly Container zoomedContent;
    protected override Container<Drawable> Content => zoomedContent;

    private readonly LayoutValue zoomedContentWidthCache = new(Invalidation.DrawSize);

    private float currentZoom = 1;
    private float minZoom;
    private float maxZoom;

    public ZoomableScrollContainer(Direction direction = Direction.Horizontal) : base(direction)
    {
        base.Content.Add(zoomedContent = new Container
        {
            RelativeSizeAxes = Axes.Y,
            Alpha = 0,
        });

        AddLayout(zoomedContentWidthCache);
    }

    public void SetupZoom(float initial, float minimum, float maximum)
    {
        minZoom = minimum;
        maxZoom = maximum;

        currentZoom = initial;
        zoomedContentWidthCache.Invalidate();

        zoomedContent.Show();
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        if (!e.ControlPressed) return base.OnScroll(e);

        if (editorClock?.IsRunning == true)
        {
            editorClock.Stop();
        }

        var newZoom = Math.Clamp(currentZoom + e.ScrollDelta.Y * (maxZoom - minZoom) * zoom_speed, minZoom, maxZoom);
        var focusPoint = zoomedContent.ToLocalSpace(ToScreenSpace(new Vector2(DrawWidth / 2, 0))).X;
        float focusOffset = focusPoint - (float)Current;
        float expectedWidth = DrawWidth * newZoom;
        float targetOffset = expectedWidth * (focusPoint / zoomedContent.DrawWidth) - focusOffset;

        currentZoom = newZoom;
        updateZoomedContentWidth();

        Invalidate(Invalidation.DrawSize);
        ScrollTo(targetOffset, false);

        return true;
    }

    protected override void Update()
    {
        base.Update();

        Content.Margin = new MarginPadding { Horizontal = DrawWidth / 2 };
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (!zoomedContentWidthCache.IsValid)
            updateZoomedContentWidth();
    }

    private void updateZoomedContentWidth()
    {
        zoomedContent.Width = DrawWidth * currentZoom;
        zoomedContentWidthCache.Validate();
    }
}

public partial class ZoomableScrollContainer<T> : ScrollContainer<T>
    where T : Drawable
{
    protected ZoomableScrollContainer(Direction direction) : base(direction)
    {
    }

    protected override ScrollbarContainer CreateScrollbar(Direction direction)
    {
        return new ZoomableScrollbarContainer(direction);
    }

    protected partial class ZoomableScrollbarContainer(Direction direction) : ScrollbarContainer(direction)
    {
        public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
        {
        }
    }
}
