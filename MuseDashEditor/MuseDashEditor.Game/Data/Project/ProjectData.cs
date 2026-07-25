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

namespace MuseDashEditor.Game.Data.Project;

public class ProjectData(string? importedChartHash = null)
{
    /// <summary>
    /// A custom name for the project. If left empty, the name of the chart will be used. Useful for multiple charts with the same name.
    /// </summary>
    public string? CustomName { get; set; }

    /// <summary>
    /// The hash of the imported chart, if the project was imported from a chart. Used to avoid re-importing the same chart.
    /// </summary>
    public string? ImportedChartHash { get; set; } = importedChartHash;
}
