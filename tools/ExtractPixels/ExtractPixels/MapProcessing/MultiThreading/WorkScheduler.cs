using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.MultiThreading;

public class WorkScheduler<DataClass, WorkerClass>
        where DataClass : WorkerData
        where WorkerClass : Worker<DataClass>, new()
{
    public int NbThreads = 1500;
    public int PoolingTimeMilliSeconds = 100;
    private List<WorkerClass> _workers = new List<WorkerClass>();
    public void Run(List<DataClass> workerData)
    {
        foreach (var data in workerData)
        {
            var worker = new WorkerClass();
            worker.Initialize(data);
            _workers.Add(worker);
        }

        while (!WorksCompleted())
        {
            TryExecNewThreads();
            Thread.Sleep(PoolingTimeMilliSeconds);
        }
        var exception = GetEventualException();
        if (exception != null) { throw exception; }
    }

    private void TryExecNewThreads()
    {
        var workers = GetNextWorkerNotStated();
        foreach (var worker in workers)
        {
            var parameterizedThreadStart = new ParameterizedThreadStart(ExecWorker);
            var thread = new Thread(parameterizedThreadStart);
            thread.Start(worker);
        }
    }
    private bool WorksCompleted()
    {
        lock (_workers)
        {
            return !_workers.Any(m => m.WorkerData.WorkerState != WorkerState.Completed);
        }
    }
    private List<WorkerClass> GetNextWorkerNotStated()
    {
        lock (_workers)
        {
            var nbRunningWorkers = _workers.Count(m => m.WorkerData.WorkerState == WorkerState.Running);
            var nbWorkersToTake = NbThreads - nbRunningWorkers < 0 ? 0 : NbThreads - nbRunningWorkers;
            return _workers
                .Where(m => m.WorkerData.WorkerState == WorkerState.NotStarted)
                .Take(nbWorkersToTake).ToList();
        }
    }
    private Exception GetEventualException()
    {
        lock (_workers)
        {
            return _workers.FirstOrDefault(m => m.WorkerData.Exception != null)?.WorkerData?.Exception;
        }
    }
    private void ExecWorker(object workerAsObject)
    {
        if (workerAsObject == null) { return; }
        WorkerClass worker = (WorkerClass)workerAsObject;
        worker.Run();
    }

}

