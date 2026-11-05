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
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MuseDashEditor.Game.Conversion.BmsFormat;
using MuseDashEditor.Game.Conversion.Converter;
using MuseDashEditor.Game.Conversion.MdeFormat;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Project;
using osu.Framework.Allocation;
using osu.Framework.Platform;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MuseDashEditor.Game.Project;

public partial class ProjectManager : IDependencyInjectionCandidate
{
    [Resolved]
    protected GameHost Host { get; private set; } = null!;

    [Resolved]
    protected EditorDataHolder DataHolder { get; private set; } = null!;

    public IEnumerable<Storage> GetAllProjects()
    {
        var projectsStorage = Host.Storage.GetStorageForDirectory("projects");
        return projectsStorage.GetDirectories(".")
                              .Select(projectsStorage.GetStorageForDirectory)
                              .Where(storage => storage.Exists("project.mdep"));
    }

    public Storage CreateProject(string importedChartHash = "")
    {
        var projectsStorage = Host.Storage.GetStorageForDirectory("projects");

        var projectUid = Guid.NewGuid();
        var projectStorage = projectsStorage.GetStorageForDirectory(projectUid.ToString());

        var projectData = new ProjectData(importedChartHash);
        using var stream = projectStorage.CreateFileSafely("project.mdep");
        stream.Write(JsonSerializer.SerializeToUtf8Bytes(projectData, typeof(ProjectData)));

        return projectStorage;
    }

    public async Task<Storage?> ImportChart(string pathValueFullName, bool convertToMde = false)
    {
        var inputFileInfo = new FileInfo(pathValueFullName);
        if (!inputFileInfo.Exists)
            return null;

        string fileHash;

        using (var hasher = SHA1.Create())
        {
            await using (var stream = File.OpenRead(pathValueFullName))
            {
                var hash = await hasher.ComputeHashAsync(stream);
                fileHash = BitConverter.ToString(hash).Replace("-", "");
            }
        }

        var storage = CreateProject(fileHash);
        var storagePath = storage.GetFullPath(".");

        await using var sourceStream = inputFileInfo.OpenRead();
        ZipFile.ExtractToDirectory(sourceStream, storagePath);

        if (convertToMde)
            await BmsToMdeConverter.ConvertFromBms(storage);

        return storage;
    }

    public static async Task Save(EditorDataHolder dataHolder)
    {
        var chart = dataHolder.CurrentChart.Value;
        if (chart is null)
            return;

        var infoFile = new FileInfo(Path.Combine(chart.Directory.FullName, "info.json"));
        var data = JsonSerializer.Serialize(chart.ChartInfo.ToRaw());
        await File.WriteAllTextAsync(infoFile.FullName, data, Encoding.UTF8);

        var map = dataHolder.CurrentMap.Value;
        if (map is null)
            return;

        switch (map.Type)
        {
            case MapType.Bms:
                await BmsMapParser.Save(map.MapFile, map);
                break;

            case MapType.Mdem:
                await MdeMapParser.Save(map.MapFile, map);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task<Storage> CreateNewProject(FileInfo audioFile)
    {
        var storage = CreateProject();

        var musicFilePath = storage.GetFullPath($"music{audioFile.Extension}");
        audioFile.CopyTo(musicFilePath);

        DataHolder.CurrentChart.Value.Directory = new DirectoryInfo(storage.GetFullPath("."));

        await Save(DataHolder);

        return storage;
    }
}
