using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.MultiThreading;

public enum WorkerState
{
    NotStarted = 0,
    Running = 1,
    Completed = 2
}

