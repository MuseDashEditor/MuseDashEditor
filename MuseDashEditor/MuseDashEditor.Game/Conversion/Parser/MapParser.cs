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
using MuseDashEditor.Game.Data.Chart;

namespace MuseDashEditor.Game.Conversion.Parser;

public abstract class MapParser
{
    public abstract Task<Map?> Parse(FileInfo file);

    public static MapParser? GetParser(FileInfo file)
    {
        return file.Extension switch
        {
            ".bms" => new BmsMapParser(),
            ".mdem" => new MdeMapParser(),
            _ => null
        };
    }
}
