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

using System.IO;
using System.Threading.Tasks;
using MuseDashEditor.Game.Conversion.BmsFormat;
using MuseDashEditor.Game.Conversion.MdeFormat;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace MuseDashEditor.Game.Conversion.Converter;

public static class BmsToMdeConverter
{
    public static async Task ConvertFromBms(Storage storage)
    {
        Logger.Log("Converting maps from BMS to MDEM...");

        var parser = new BmsMapParser();

        foreach (var filePath in storage.GetFiles(".", "*.bms"))
        {
            var oldFilePath = storage.GetFullPath(filePath);

            Logger.Log($"Converting from {oldFilePath}...");

            var fileInfo = new FileInfo(oldFilePath);
            if (!fileInfo.Exists)
                continue;

            var mapName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
            var newFileName = $"{mapName}.mdem";

            var newFile = new FileInfo(storage.GetFullPath(newFileName));

            await parser.Parse(fileInfo)
                        .ContinueWith(task =>
                        {
                            if (!task.IsCompletedSuccessfully || task.Result is null)
                                return Task.CompletedTask;

                            return MdeMapParser.Save(newFile, task.Result);
                        });

            storage.Delete(filePath);

            Logger.Log($"Converted to {newFileName}!");
        }
    }
}
