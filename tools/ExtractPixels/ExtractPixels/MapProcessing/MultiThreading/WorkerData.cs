using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.MultiThreading;

public class WorkerData
{
    public Exception Exception { get; set; } = null;
    public WorkerState WorkerState { get; set; }
    public string WorkerName { get; set; }


}

