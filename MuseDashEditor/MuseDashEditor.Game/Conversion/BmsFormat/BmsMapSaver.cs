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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Object;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Object.MappingObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Utils;
using osu.Framework.Utils;
using MathUtils = MuseDashEditor.Game.Utils.MathUtils;

namespace MuseDashEditor.Game.Conversion.BmsFormat;

public static class BmsMapSaver
{
    public static async Task Save(FileInfo file, Map map)
    {
        await File.WriteAllTextAsync(file.FullName, GetSaveContent(map), Encoding.UTF8);
    }

    public static string GetSaveContent(Map map)
    {
        var (header, addedBpms) = buildHeader(map);
        var content = buildContent(map, addedBpms);

        return (header + content).Trim();
    }

    private static (string header, Dictionary<int, double>? addedBpms) buildHeader(Map map)
    {
        var header = $"""
                      *---------------------- HEADER FIELD

                      #PLAYER {(int)map.Metadata.InitialLaneSpeed.Value}
                      #GENRE scene_{(int)map.Metadata.InitialScene.Value:D2}
                      #TITLE
                      #ARTIST
                      #LEVELDESIGN
                      #BPM {map.Metadata.InitialBpm.Value.ToString(CultureInfo.InvariantCulture)}
                      #PLAYLEVEL 1
                      #RANK 1
                      #LNTYPE 1


                      """;

        if (map.TimingPoints.Count <= 1)
            return (header, null);

        var addedBpms = new Dictionary<int, double>();
        var bpmIndex = 1;

        for (var i = 1; i < map.TimingPoints.Count; i++)
        {
            var newBpmValue = map.TimingPoints[i].NewBpm.Value;

            if (!addedBpms.TryAdd(bpmIndex, newBpmValue))
                continue;

            header += $"#BPM{Base36Converter.ToBase36(bpmIndex++)} {newBpmValue.ToString(CultureInfo.InvariantCulture)}{Environment.NewLine}";
        }

        header += Environment.NewLine;

        return (header, addedBpms);
    }

    private static string buildContent(Map map, Dictionary<int, double>? addedBpms)
    {
        var content = """
                      *---------------------- MAIN DATA FIELD
                      #00001:10


                      """;

        var currentTimingPoint = map.TimingPoints[0];

        var elapsedDuration = 0D;

        for (int track = 0; track < 1000; track++)
        {
            var trackStartTime = elapsedDuration;
            var trackLength = 60 / currentTimingPoint.NewBpm.Value * 4000;

            var timeSignature = map.TimeSignatures.FirstOrDefault(o => o.Index.Value == track);
            var signature = timeSignature?.NewSignature?.Value ?? 1.0D;

            trackLength *= signature;

            var trackEndTime = elapsedDuration + trackLength;

            var localTrackId = track;
            List<TimingPointObject> timingPointsInTrack = [.. map.TimingPoints.Where(tp => tp.Index.Value > 0 && tp.ObjectTick.Value.TrackIndex == localTrackId)];
            var timingPointsInTrackLcm = MathUtils.Lcm([.. timingPointsInTrack.Select(o => o.ObjectTick.Value.TrackDivisor)]);
            var timingPointsInTrackGcd = MathUtils.Gcd([.. timingPointsInTrack.Select(o => o.ObjectTick.Value.TrackTickIndex)]);
            var timingPointsGlobalGcd = MathUtils.Gcd(timingPointsInTrackLcm, timingPointsInTrackGcd);
            timingPointsInTrackLcm /= timingPointsGlobalGcd;

            foreach (var nextTimingPoint in timingPointsInTrack)
            {
                var offsetValue = nextTimingPoint.Offset.Value;

                var isAfterTrackStart = Precision.AlmostEquals(offsetValue, trackStartTime, 0.1f) || offsetValue >= trackStartTime;
                var isBeforeTrackEnd = offsetValue < trackEndTime && !Precision.AlmostEquals(offsetValue, trackEndTime, 0.1f);

                if (!isAfterTrackStart || !isBeforeTrackEnd)
                    continue;

                var (_, divisor, beatIndex) = nextTimingPoint.ObjectTick.Value;
                var tickLength = trackLength / timingPointsInTrackLcm;
                var objectTickInTrack = (beatIndex * timingPointsInTrackLcm) / divisor;

                var totalTicksInTrack = trackLength / tickLength;
                var remainingTicks = totalTicksInTrack - objectTickInTrack;

                var newTrackLength = 60 / nextTimingPoint.NewBpm.Value * 4000 * signature;
                var newTickLength = newTrackLength / totalTicksInTrack;

                var newRemainingTime = remainingTicks * newTickLength;

                trackEndTime = offsetValue + newRemainingTime;
            }

            currentTimingPoint = timingPointsInTrack.LastOrDefault(currentTimingPoint);

            trackLength = trackEndTime - trackStartTime;

            if (timingPointsInTrack.Count > 0)
            {
                var trackData = new List<string>(timingPointsInTrackLcm);

                for (int i = 0; i < timingPointsInTrackLcm; i++)
                {
                    trackData.Add("00");
                }

                foreach (var timingPointObject in timingPointsInTrack)
                {
                    var objectTick = timingPointObject.ObjectTick.Value;
                    var tick = (objectTick.TrackTickIndex * timingPointsInTrackLcm) / objectTick.TrackDivisor;

                    var index = addedBpms?.First(pair => Precision.AlmostEquals(pair.Value, timingPointObject.NewBpm.Value)).Key ?? timingPointObject.Index.Value;

                    trackData[tick] = Base36Converter.ToBase36(index);
                }

                content += $"#{track:000}08:{string.Join("", trackData)}{Environment.NewLine}";
            }

            var timeSigObject = map.TimeSignatures.FirstOrDefault(o => o.ObjectTick.Value.TrackIndex == track);

            if (timeSigObject is not null)
            {
                content += $"#{track:000}02:{timeSigObject.NewSignature.Value.ToString(CultureInfo.InvariantCulture)}{Environment.NewLine}";
            }

            Dictionary<(LaneModifierType, LaneType), List<BaseObject>> objectsPerChannel = [];

            var objectsInTrack = map.GameObjects.Where(o => o.ObjectTick.Value.TrackIndex == track).ToList();

            foreach (var laneModifier in Enum.GetValues<LaneModifierType>())
            {
                foreach (var laneType in Enum.GetValues<LaneType>())
                {
                    foreach (var gameObject in objectsInTrack.Where(o => o.LaneModifier == laneModifier && o.LaneType == laneType))
                    {
                        objectsPerChannel.ComputeIfAbsent((laneModifier, laneType), () => []).Add(gameObject);
                    }
                }
            }

            var writtenOneLine = false;

            foreach (var (key, objects) in objectsPerChannel)
            {
                var channel = $"{Base36Converter.ToBase36((int)key.Item1)[1]}" +
                              $"{Base36Converter.ToBase36((int)key.Item2)[1]}";

                var lcm = MathUtils.Lcm([.. objects.Select(o => o.ObjectTick.Value.TrackDivisor)]);
                var gcd = MathUtils.Gcd(MathUtils.Gcd([.. objects.Select(o => o.ObjectTick.Value.TrackTickIndex)]), lcm);

                var divisor = lcm / gcd;

                var trackData = new List<string>(divisor);

                for (int i = 0; i < divisor; i++)
                {
                    trackData.Add("00");
                }

                foreach (var obj in objects)
                {
                    var objectTick = obj.ObjectTick.Value;
                    var tick = objectTick.TrackTickIndex * (lcm / objectTick.TrackDivisor) / gcd;

                    trackData[tick] = obj switch
                    {
                        GameObject gameObject => Base36Converter.ToBase36((int)gameObject.ObjectType),
                        _ => trackData[tick]
                    };
                }

                content += $"#{track:000}{channel}:{string.Join("", trackData)}{Environment.NewLine}";
                writtenOneLine = true;
            }

            if (writtenOneLine)
                content += Environment.NewLine;

            elapsedDuration += trackLength;

            if (map.GameObjects.Count > 0 && Precision.DefinitelyBigger(elapsedDuration, map.GameObjects[^1].Offset.Value, 1))
            {
                break;
            }
        }

        return content;
    }
}
