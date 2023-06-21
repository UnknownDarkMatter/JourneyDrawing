namespace ExtractPixels.FileAdapter;

public class CsvFile
{
    public string ColumnSeparator { get; set; }
    public string Content { get; set; }
    /// <summary>
    /// Name without extension that can be used as the name of an Excel sheet
    /// </summary>
    public string Name { get; set; }
}