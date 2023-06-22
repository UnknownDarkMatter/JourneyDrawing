using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels;

public static class Constants
{
    public const bool IsDebug = true;
    public const bool IsDebugImage = false;
    /// <summary>
    /// If false, the program look for trips beeteen all points to all points.
    /// If true, the program look for trips beetween only all the ports to the ports.
    /// </summary>
    public const bool UsePortDeclaration = false;


    public const string CsvColumnSeparatorEscape="\"";
}
