using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.FileAdapter;

public class CsvDateFormats
{
    public IEnumerable<CsvDateFormat> DateFormats { get; set; }

    public CsvDateFormats(IEnumerable<CsvDateFormat> dateFormats)
    {
        DateFormats = dateFormats;
    }

    public string? GetFormat(string name)
    {
        return DateFormats.FirstOrDefault(m => m.Name == name)?.Format;
    }
}
