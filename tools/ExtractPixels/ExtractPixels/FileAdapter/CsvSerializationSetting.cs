using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.FileAdapter;

public class CsvSerializationSetting
{
    /// <summary>
    /// column name in the Csv file
    /// </summary>
    public string ColumnName { get; set; }
    /// <summary>
    /// Name for reflection when deserializing csv to typed object (name of property selected with "ValueGet")
    /// </summary>
    public string PropertyName { get; set; }
    public DataType DataType { get; set; }

    public delegate dynamic GetValueDelegate(dynamic object2Convert);

    public GetValueDelegate ValueGet { get; set; }

}
