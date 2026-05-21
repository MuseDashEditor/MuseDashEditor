using MuseDashEditor.Game.Data.Holder;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace MuseDashEditor.Game.Editor.Clock;

public partial class EditorClock : CompositeComponent, IFrameBasedClock, IAdjustableClock, ISourceChangeableClock
{
    private readonly DecouplingFramedClock decouplingClock;
    private readonly IFrameBasedClock interpolatingClock;
    private double currentTrackLength;

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

        if (CurrentTime > currentTrackLength)
            Seek(currentTrackLength);

        interpolatingClock.ProcessFrame();
    }

    public void Reset()
    {
        decouplingClock.Reset();
        interpolatingClock.ProcessFrame();
    }

    public bool Seek(double position)
    {
        if (position < 0)
            return false;
        if (position > currentTrackLength)
            return false;

        if (IsRunning) Stop(); // Maybe add an option to keep running?

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

    public void ChangeSource(IClock source)
    {
        if (source is Track track)
        {
            currentTrackLength = track.Length;
        }
        else
        {
            currentTrackLength = 0;
        }

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
