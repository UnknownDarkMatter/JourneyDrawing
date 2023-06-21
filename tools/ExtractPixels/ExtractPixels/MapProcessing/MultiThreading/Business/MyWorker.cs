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
            TripGenerator.ComputationCount++;

            var rest = (int)(TripGenerator.ComputationCount % (TripGenerator.MaxCount * 0.1M));
            if (rest == 0 || rest == (TripGenerator.MaxCount * 0.1M))
            {
                Console.WriteLine($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} : DONE {(int)(100 * (((decimal)TripGenerator.ComputationCount / (decimal)TripGenerator.MaxCount)))}%.");
            }
        }
        catch (Exception ex)
        {
            WorkerData.Exception = ex;
        }
        WorkerData.WorkerState = WorkerState.Completed;
    }
}
