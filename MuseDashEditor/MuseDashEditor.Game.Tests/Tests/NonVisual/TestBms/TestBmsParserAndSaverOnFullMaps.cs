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
using System.IO;
using System.Linq;
using MuseDashEditor.Game.Conversion.BmsFormat;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Tests.Resources;
using NUnit.Framework;
using MathUtils = MuseDashEditor.Game.Utils.MathUtils;

namespace MuseDashEditor.Game.Tests.Tests.NonVisual.TestBms;

public class TestBmsParserAndSaverOnFullMaps
{
    [Theory]
    public void Parse([Range(0, 172)] int mapId)
    {
        // Maps not included on VCS
        using var inputBmsStream = TestResources.OpenResource($"TestBmsParserAndSaver/FullMaps/map_{mapId}.bms");

        Assert.NotNull(inputBmsStream);

        using var inputBmsStreamReader = new StreamReader(inputBmsStream);

        var inputBmsContent = inputBmsStreamReader.ReadToEnd().ReplaceLineEndings(Environment.NewLine);

        Map map = new(null!, MapType.Bms);

        BmsMapParser.ParseMapContent(inputBmsContent, map);
        BmsMapParser.ProcessMapContent(map);

        var saveContent = BmsMapParser.GetSaveContent(map);

        var hasSeenDataHeaderInInput = true;
        var inputLines = inputBmsContent.Split(Environment.NewLine)
                                        .Select(s => s.Trim())
                                        .Where(s => s.Length > 0)
                                        .SkipWhile(s =>
                                        {
                                            if (s.Contains("DATA FIELD"))
                                                hasSeenDataHeaderInInput = false;
                                            return hasSeenDataHeaderInInput;
                                        })
                                        .ToList();

        var hasSeenDataHeaderInOutput = true;
        var outputLines = saveContent.Split(Environment.NewLine)
                                     .Select(s => s.Trim())
                                     .Where(s => s.Length > 0)
                                     .SkipWhile(s =>
                                     {
                                         if (s.Contains("DATA FIELD"))
                                             hasSeenDataHeaderInOutput = false;
                                         return hasSeenDataHeaderInOutput;
                                     })
                                     .ToList();

        Assert.Multiple(() =>
        {
            Assert.AreEqual(inputLines.Count, outputLines.Count);

            for (int lineIndex = 0; lineIndex < Math.Min(inputLines.Count, outputLines.Count); lineIndex++)
            {
                var inputLine = inputLines[lineIndex];
                var outputLine = outputLines[lineIndex];

                if (outputLine == inputLine)
                {
                    Assert.AreEqual(outputLine, inputLine);
                    continue;
                }

                checkSameWithDifferentDivisors(inputLine, outputLine);
            }
        });
    }

    private static void checkSameWithDifferentDivisors(string inputLine, string outputLine)
    {
        var (inputDivisor, inputData) = extractData(inputLine);
        var (outputDivisor, outputData) = extractData(outputLine);

        var lcm = MathUtils.Lcm(inputDivisor, outputDivisor);
        var inputLcmDivisor = lcm / inputDivisor;
        var outputLcmDivisor = lcm / outputDivisor;

        List<(int, string)> convertedInputData = [];
        List<(int, string)> convertedOutputData = [];

        foreach (var (index, data) in inputData)
        {
            var convertedIndex = index * inputLcmDivisor;
            convertedInputData.Add((convertedIndex, data));
        }

        foreach (var (index, data) in outputData)
        {
            var convertedIndex = index * outputLcmDivisor;
            convertedOutputData.Add((convertedIndex, data));
        }

        if (convertedInputData.Count != convertedOutputData.Count)
        {
            Assert.AreEqual(outputLine, inputLine); // Display line diff
            return;
        }

        Assert.AreEqual(convertedInputData, convertedOutputData);
    }

    private static (int, List<(int, string)>) extractData(string line)
    {
        line = line.Split(":")[1];

        if (line.Length % 2 != 0)
            throw new ArgumentException("Invalid line length", nameof(line));

        List<string> objectsData = [];

        for (int i = 0; i < line.Length; i += 2)
        {
            objectsData.Add(line[i] + "" + line[i + 1]);
        }

        var dataCount = objectsData.Count;

        List<(int, string)> res = [];

        for (int index = 0; index < dataCount; index++)
        {
            var data = objectsData[index];

            if (data == "00")
                continue;

            res.Add((index, data));
        }

        return (dataCount, res);
    }
}
