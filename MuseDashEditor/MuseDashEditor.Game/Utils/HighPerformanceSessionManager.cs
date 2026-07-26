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
using System.Runtime;

namespace MuseDashEditor.Game.Utils;

public class HighPerformanceSessionManager
{
    private GCLatencyMode originalGcMode;
    private bool isActive;

    public void Start()
    {
        if (isActive) return;
        isActive = true;
        originalGcMode = GCSettings.LatencyMode;
        GCSettings.LatencyMode = GCLatencyMode.LowLatency;
        GC.Collect(0);
    }

    public void Stop()
    {
        if (!isActive) return;
        isActive = false;
        GCSettings.LatencyMode = originalGcMode;
    }
}
