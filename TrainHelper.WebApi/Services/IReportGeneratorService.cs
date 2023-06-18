using TrainHelper.WebApi.Dto;

namespace TrainHelper.WebApi.Services;

public interface IReportGeneratorService 
{
    Task<NlDetailsReportDto?> GetNlDetailsReport(int trainNumber);
    Task<byte[]> GetNlDetailsReportXlsx(int trainNumber);
}