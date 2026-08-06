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
using System.Threading.Tasks;
using MuseDashEditor.Game.Conversion.MdeFormat;
using MuseDashEditor.Game.Data.Project;
using osu.Framework.Allocation;
using osu.Framework.Platform;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MuseDashEditor.Game.Project;

public partial class ProjectManager : IDependencyInjectionCandidate
{
    [Resolved] protected GameHost Host { get; private set; } = null!;

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

    public async Task<Storage?> ImportChart(string pathValueFullName)
    {
        var inputFileInfo = new FileInfo(pathValueFullName);
        if (!inputFileInfo.Exists)
            return null;

        string fileHash;
        using (var hasher = System.Security.Cryptography.SHA1.Create())
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

        await MdeChartLoader.ConvertFromBms(storage);

        return storage;
    }
}
