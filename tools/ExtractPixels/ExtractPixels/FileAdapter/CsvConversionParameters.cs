using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.FileAdapter;

public class CsvConversionParameters
{
    public CsvDateFormats CsvDateFormats { get; set; }
    public char CsvColumnSeparator { get; set; }
    /// <summary>
    /// Escape the column separator to have column separator in the value of the cell.
    /// It is at the beginning and the end of the cell value
    /// </summary>
    public string CsvColumnSeparatorEscape { get; set; } = Constants.CsvColumnSeparatorEscape;
    public string CsvDecimalCulture { get; set; }
}
