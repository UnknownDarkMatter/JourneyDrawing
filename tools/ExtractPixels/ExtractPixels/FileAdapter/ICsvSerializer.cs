using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.FileAdapter;

public interface ICsvSerializer
{
    public IEnumerable<T> DeserializeFromCsv<T>(CsvFile csvFile, string listName);
    public CsvFile SerializeToCsv<T>(T[] objectsList, string listName)
        where T : new();
}
