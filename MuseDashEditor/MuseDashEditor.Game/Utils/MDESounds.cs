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
using ManagedBass.Fx;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Mixing;
using osu.Framework.Audio.Sample;

namespace MuseDashEditor.Game.Utils;

public partial class MdeSounds : IDependencyInjectionCandidate
{
    [Resolved] protected ISampleStore SampleStore { get; private set; } = null!;
    [Resolved] protected AudioManager AudioManager { get; private set; } = null!;

    private const int concurrent_samples = 10;
    private readonly Dictionary<HitSoundType, Sample> samples = new();
    private readonly Dictionary<HitSoundType, Queue<SampleChannel>> preloadedChannels = new();
    private readonly Dictionary<HitSoundType, AudioMixer> hitSoundsMixers = new();

    public void Preload()
    {
        foreach (HitSoundType hitSoundType in Enum.GetValuesAsUnderlyingType<HitSoundType>())
        {
            if (hitSoundType == HitSoundType.None)
                continue;

            var sampleName = Enum.GetName(typeof(HitSoundType), hitSoundType)?.ToLowerInvariant();
            var sample = SampleStore.Get($"Hit/{sampleName}");
            sample.PlaybackConcurrency.Value = concurrent_samples;

            samples.Add(hitSoundType, sample);

            preloadedChannels.Add(hitSoundType, new Queue<SampleChannel>(concurrent_samples));
            var audioMixer = AudioManager.CreateAudioMixer("HitSound-" + sampleName);
            hitSoundsMixers.Add(hitSoundType, audioMixer);
            hitSoundsMixers[hitSoundType].AddEffect(new CompressorParameters
            {
                fGain = 0
            });

            for (int i = 0; i < concurrent_samples; i++)
            {
                var sampleChannel = sample.GetChannel();
                audioMixer.Add(sampleChannel);
                preloadedChannels[hitSoundType].Enqueue(sampleChannel);
            }
        }
    }

    public void Dispose()
    {
        foreach (var audioMixer in hitSoundsMixers.Values)
        {
            audioMixer.Dispose();
        }

        foreach (var sampleChannel in preloadedChannels.Values.SelectMany(sampleChannel => sampleChannel))
        {
            sampleChannel.Dispose();
        }
    }

    public void PlayHitSound(HitSoundType hitSoundType)
    {
        if (hitSoundType == HitSoundType.None)
            return;

        preloadedChannels[hitSoundType].Dequeue().Play();

        var sample = samples[hitSoundType];
        var newSample = sample.GetChannel();
        hitSoundsMixers[hitSoundType].Add(newSample);
        preloadedChannels[hitSoundType].Enqueue(newSample);
    }
}
