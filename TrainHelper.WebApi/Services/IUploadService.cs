using TrainHelper.WebApi.Dto;

namespace TrainHelper.WebApi.Services;

public interface IUploadService 
{
    Task<UploadDataResultDto> UploadData(UploadFileDto uploadFile);
}