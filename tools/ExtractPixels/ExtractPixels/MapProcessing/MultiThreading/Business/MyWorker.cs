using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels.MapProcessing.MultiThreading.Business;

public class MyWorker : Worker<MyData>
{
    private TripGenerator _tripGenerator;
    public override void Initialize(MyData workerData)
    {
        WorkerData = workerData;

        _tripGenerator = new TripGenerator();
    }

    public override void Run()
    {
        WorkerData.WorkerState = WorkerState.Running;

        try
        {
            var seaTrip = _tripGenerator.CalculateSingleTrip(WorkerData.sStart, WorkerData.sEnd, 
                WorkerData.borderWalkingPoints, WorkerData.width, WorkerData.height, WorkerData.imageFilePath, WorkerData.image);

            WorkerData.SeaTrip = seaTrip;

        }
        catch (Exception ex)
        {
            WorkerData.Exception = ex;
        }
        WorkerData.WorkerState = WorkerState.Completed;
    }
}
