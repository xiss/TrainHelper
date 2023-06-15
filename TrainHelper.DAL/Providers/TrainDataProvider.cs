using Microsoft.EntityFrameworkCore;
using TrainHelper.DAL.Entities;

namespace TrainHelper.DAL.Providers;

public class TrainDataProvider : IDisposable, ITrainDataProvider
{
    private readonly DataContext _dataContext;

    public TrainDataProvider(DataContext dataContext) => _dataContext = dataContext;

    public async Task<bool> AddTrain(Train train)
    {
        try
        {
            //TODO Поправить все это
            //Check train to exist
            var newTrain = await _dataContext.Trains
                .Include(t => t.Cars)
                .ThenInclude(c => c.WayPoints)
                .FirstOrDefaultAsync(t => t.Number == train.Number);

            if (newTrain == null)
            {
                newTrain = new Train() { Number = train.Number, TrainIndexCombined = train.TrainIndexCombined };
                await _dataContext.Trains.AddAsync(newTrain);
            }

            // Check invoice to exist
            var invoice = await _dataContext.Invoices.FirstOrDefaultAsync(i => i.InvoiceNum == train.Cars.Single().Invoice.InvoiceNum)
                          ?? train.Cars.Single().Invoice;

            // Check freight to exist
            var freight = await _dataContext.Freights.FirstOrDefaultAsync(f => f.FreightEtsngName == train.Cars.Single().Freight.FreightEtsngName)
                          ?? train.Cars.Single().Freight;

            // Check car operation to exist
            var carOperation = await _dataContext.CarOperations.FirstOrDefaultAsync(c =>
                c.OperationName == train.Cars.Single().WayPoints.Single().Operation.OperationName) ?? train.Cars.Single().WayPoints.Single().Operation;

            // Check last station to exist
            var lastStation = await _dataContext.Stations.FirstOrDefaultAsync(s => s.StationName == train.Cars.Single().WayPoints.Single().Station.StationName)
                              ?? train.Cars.Single().WayPoints.Single().Station;

            // Check cars to exist

            var car = newTrain.Cars.FirstOrDefault(c => c.CarNumber == train.Cars.Single().CarNumber);
            if (car == null)
            {
                car = train.Cars.Single();
                car.Invoice = invoice;
                car.Freight = freight;
                car.FromStation = await _dataContext.Stations.FirstOrDefaultAsync(s => s.StationName == train.Cars.Single().FromStation.StationName)
                                  ?? train.Cars.Single().FromStation;
                car.ToStation = await _dataContext.Stations.FirstOrDefaultAsync(s => s.StationName == train.Cars.Single().ToStation.StationName)
                                  ?? train.Cars.Single().ToStation;
                newTrain.Cars.Add(car);
            }

            //Check way point to exist
            train.Cars.Single().WayPoints.Single().Operation = carOperation;
            train.Cars.Single().WayPoints.Single().Station = lastStation;

            await _dataContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public void Dispose() => _dataContext.Dispose();

    public async Task<Train?> GetTrainDetail(int trainNumber) => await _dataContext.Trains
            .Include(t => t.Cars)
            .ThenInclude(c => c.Invoice)
            .Include(t => t.Cars)
            .ThenInclude(c => c.Freight)
            .Include(t => t.Cars)
            .ThenInclude(c => c.FromStation)
            .Include(t => t.Cars)
            .ThenInclude(c => c.ToStation)
            .Include(t => t.Cars)
            .ThenInclude(c => c.WayPoints.OrderByDescending(w => w.OperationDate).Take(1))
            .ThenInclude(w => w.Operation)
            .Include(t => t.Cars)
            .ThenInclude(c => c.WayPoints)
            .ThenInclude(w => w.Station)
            .FirstOrDefaultAsync(t => t.Number == trainNumber);
}