using TrainHelper.DAL.Entities;

namespace TrainHelper.DAL.Providers;

public interface ITrainDataProvider
{
    Task<bool> UpsertTrain(Train train);
    void Dispose();
    Task<Train?> GetTrainDetail(int trainNumber);
}