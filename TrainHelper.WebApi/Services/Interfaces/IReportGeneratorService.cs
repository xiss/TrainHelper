using TrainHelper.WebApi.Dto;

namespace TrainHelper.WebApi.Services.Interfaces
{
    public interface IReportGeneratorService : IDisposable
    {
        new void Dispose();
        Task<NlDetailsReportDto?> GetNlDetailsReport(int trainNumber);
        Task<byte[]> GetNlDetailsReportXlsx(int trainNumber);
    }
}