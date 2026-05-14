using JetBrains.Annotations;
using MuseDashEditor.Game.Data.Holder;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace MuseDashEditor.Game.Editor.Clock;

public partial class EditorClock : CompositeComponent, IFrameBasedClock, IAdjustableClock, ISourceChangeableClock
{
    private readonly DecouplingFramedClock decouplingClock;
    private readonly IFrameBasedClock interpolatingClock;

    public EditorClock()
    {
        decouplingClock = new DecouplingFramedClock();
        interpolatingClock = new InterpolatingFramedClock(decouplingClock);
    }

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        dataHolder.CurrentTrack.BindValueChanged(track =>
        {
            Stop();
            ChangeSource(track.NewValue);
        }, true);
    }

    public void Start()
    {
        decouplingClock.Start();
        interpolatingClock.ProcessFrame();
    }

    public void Stop()
    {
        decouplingClock.Stop();
        interpolatingClock.ProcessFrame();
    }

    public void Reset()
    {
        decouplingClock.Reset();
        interpolatingClock.ProcessFrame();
    }

    public bool Seek(double position)
    {
        var result = decouplingClock.Seek(position);
        interpolatingClock.ProcessFrame();
        return result;
    }

    public void ResetSpeedAdjustments()
    {
        decouplingClock.ResetSpeedAdjustments();
    }

    public double Rate
    {
        get => decouplingClock.Rate;
        set => decouplingClock.Rate = value;
    }

    double IClock.Rate => decouplingClock.Rate;

    protected override void Update()
    {
        base.Update();

        interpolatingClock.ProcessFrame();
    }

    public IClock Source => decouplingClock.Source;

    public void ChangeSource([CanBeNull] IClock source)
    {
        decouplingClock.ChangeSource(source);
    }

    public double CurrentTime => interpolatingClock.CurrentTime;

    public bool IsRunning => interpolatingClock.IsRunning;

    public double ElapsedFrameTime => interpolatingClock.ElapsedFrameTime;

    public double FramesPerSecond => interpolatingClock.FramesPerSecond;

    public void ProcessFrame()
    {
        // No-op
    }
}
