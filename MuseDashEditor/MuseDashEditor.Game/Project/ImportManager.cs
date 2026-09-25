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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using MuseDashEditor.Game.Component.Notification;
using osu.Framework.Allocation;

namespace MuseDashEditor.Game.Project;

public partial class ImportManager : IDependencyInjectionCandidate
{
    public static ImportManager ImportManagerInstance = new();

    private ImportManager() { }

    [Resolved]
    protected NotificationContainer NotificationContainer { get; private set; } = null!;

    public void ProcessFileDrop(List<string> paths)
    {
        if (paths.Count > 1)
        {
            // Import all
        }
        else
        {
            FileInfo fileInfo = new FileInfo(paths.First());
            if (!fileInfo.Exists)
                return;

            var fileName = fileInfo.Name;

            switch (fileInfo.Extension)
            {
                case ".mdm":
                    NotificationContainer.ShowNotification(new OpenOrImportMdmNotification(fileInfo, out _));
                    break;

                case ".mp3":
                case ".ogg":
                    // New project
                    break;

                case ".bms":
                case ".mdem":
                case ".mdep":
                case ".json":
                    validateAndPromptForImportOrOpen(fileInfo);
                    break;

                default:
                    NotificationContainer.ShowNotification(new UnknownFileTypeNotification(fileName, out _));
                    break;
            }
        }
    }

    private void validateAndPromptForImportOrOpen(FileInfo fileInfo)
    {
        var directoryInfo = fileInfo.Directory;
        if (directoryInfo is not { Exists: true })
            return;

        var infoJsonFile = new FileInfo(Path.Join(directoryInfo.FullName, "info.json"));
        var foundMapFile = false;

        for (int i = 1; i <= 4; i++)
        {
            var bmsFile = new FileInfo(Path.Join(directoryInfo.FullName, $"map{i}.bms"));
            var mdeFile = new FileInfo(Path.Join(directoryInfo.FullName, $"map{i}.mde"));

            if (!bmsFile.Exists && !mdeFile.Exists)
                continue;

            foundMapFile = true;
            break;
        }

        var mp3AudioFile = new FileInfo(Path.Join(directoryInfo.FullName, "music.mp3"));
        var oggAudioFile = new FileInfo(Path.Join(directoryInfo.FullName, "music.ogg"));
        var hasMusic = mp3AudioFile.Exists || oggAudioFile.Exists;

        var validProject = infoJsonFile.Exists && foundMapFile && hasMusic;

        if (!validProject)
        {
            NotificationContainer.ShowNotification(new InvalidProjectDirectoryNotification(directoryInfo.FullName, out _));
            return;
        }

        NotificationContainer.ShowNotification(new OpenOrImportDirectoryNotification(directoryInfo, out _));
    }

    public void OpenReadOnly(FileInfo fileInfo)
    {
        NotificationContainer.Hide();
        // TODO
        // Extract mdm to temp folder
        // Open from temp folder
    }

    public void Import(FileInfo fileInfo)
    {
        NotificationContainer.Hide();
        // TODO
        // Import from mdm
    }

    public void Open(DirectoryInfo directoryInfo)
    {
        NotificationContainer.Hide();
        // TODO
        // Open from directoryInfo
    }

    public void Import(DirectoryInfo directoryInfo)
    {
        NotificationContainer.Hide();
        // TODO
        // Import from directoryInfo
    }
}
