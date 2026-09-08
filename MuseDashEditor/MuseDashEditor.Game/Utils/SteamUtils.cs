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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Microsoft.Win32;
using MuseDashEditor.Game.Data.Chart;

namespace MuseDashEditor.Game.Utils;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public static class SteamUtils
{
    public static List<Chart> GetChartsFromGame()
    {
        var steamFolder = GetMuseDashFolder();

        if (steamFolder is null)
        {
            return [];
        }

        var customAlbumsDirectory = new DirectoryInfo(Path.Combine(steamFolder.FullName, "Custom_Albums"));
        if (!customAlbumsDirectory.Exists)
            return [];

        // Parse extracted charts
        foreach (var directoryInfo in customAlbumsDirectory.EnumerateDirectories())
        {
            var infoJsonFile = new FileInfo(Path.Combine(directoryInfo.FullName, "info.json"));
            if (!infoJsonFile.Exists)
                continue;

            // TODO
        }

        // Parse mdm files
        foreach (var fileInfo in customAlbumsDirectory.EnumerateFiles())
        {
            if (fileInfo.Extension != ".mdm")
                continue;

            var zipArchive = ZipFile.OpenRead(fileInfo.FullName);
            var infoFile = zipArchive.Entries.FirstOrDefault(entry => entry.FullName == "info.json");

            if (infoFile is null)
            {
                zipArchive.Dispose();
                continue;
            }

            var coverFile = zipArchive.Entries.FirstOrDefault(entry => entry.FullName.StartsWith("cover"));
            var musicFile = zipArchive.Entries.FirstOrDefault(entry => entry.FullName.StartsWith("musid"));
            var demoFile = zipArchive.Entries.FirstOrDefault(entry => entry.FullName.StartsWith("demo"));

            // TODO
        }

        return [];
    }

    public static DirectoryInfo? GetMuseDashFolder()
    {
        if (!OperatingSystem.IsWindows())
        {
            return null;
        }

        var steamRegistryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam");
        steamRegistryKey ??= Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam");

        var steamPath = (string?)steamRegistryKey?.GetValue("InstallPath");
        var directoryInfo = steamPath == null ? null : new DirectoryInfo(steamPath);

        if (!(directoryInfo?.Exists ?? false))
        {
            return null;
        }

        var libraryFolderPath = Path.Combine(steamPath!, "config", "libraryfolders.vdf");
        if (!File.Exists(libraryFolderPath))
            return null;

        var libraryFolders = VdfConvert.Deserialize(File.ReadAllText(libraryFolderPath));
        var libraryFolderPaths = new List<string>();

        foreach (var libraryFolderTokens in libraryFolders.Value)
        {
            if (libraryFolderTokens is not VProperty libraryFoldersProperty)
                continue;

            var libraryFolderToken = libraryFoldersProperty.Value;
            if (libraryFolderToken is not VObject libraryFolderValue)
                continue;

            foreach (var libraryFolderValueToken in libraryFolderValue.Children())
            {
                if (libraryFolderValueToken is not VProperty libraryFolderValueTokenValue)
                    continue;

                if (libraryFolderValueTokenValue.Key != "path")
                    continue;

                libraryFolderPaths.Add(libraryFolderValueTokenValue.Value.ToString());
            }
        }

        if (libraryFolderPaths.Count == 0)
            return null;

        DirectoryInfo? museDashBaseDirectory = null;

        foreach (var folderPath in libraryFolderPaths)
        {
            var libraryDirectoryInfo = new DirectoryInfo(folderPath);
            if (!libraryDirectoryInfo.Exists)
                continue;

            var libraryFolderInfo = Path.Combine(folderPath, "libraryfolder.vdf");
            if (!File.Exists(libraryFolderInfo))
                continue;

            var steamAppsDirectory = Path.Combine(libraryDirectoryInfo.FullName, "steamapps");
            if (!Directory.Exists(steamAppsDirectory))
                continue;

            var museDashManifestPath = Path.Combine(steamAppsDirectory, "appmanifest_774171.acf");
            if (!File.Exists(museDashManifestPath))
                continue;

            var museDashBaseDirectoryInfo = new DirectoryInfo(Path.Combine(steamAppsDirectory, "common", "Muse Dash"));
            if (!museDashBaseDirectoryInfo.Exists)
                continue;

            museDashBaseDirectory = museDashBaseDirectoryInfo;
            break;
        }

        return museDashBaseDirectory;
    }
}
