using System.Collections.Generic;
using System.IO;

namespace MuseDashEditor.Game.Data.Chart;

public record Map(
    FileInfo MapFile,
    Dictionary<string, string> Metadata
);
