using TrainHelper.WebApi.Dto;

namespace TrainHelper.WebApi.Services.Interfaces
{
    public interface IUploadService: IDisposable
    {
        Task<UploadDataResultDto> UploadData(IFormFile file);
    }
}