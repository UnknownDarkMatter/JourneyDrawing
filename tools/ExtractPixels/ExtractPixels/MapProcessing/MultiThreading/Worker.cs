using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.MultiThreading;

public abstract class Worker<Data> where Data : WorkerData
{
    public Data WorkerData { get; set; }

    public abstract void Initialize(Data workerData);

    public abstract void Run();

}

