namespace TrainHelper.WebApi.Dto;

public record UploadDataResultDto
{
    public int TotalItemsUploaded { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool IsSuccessful { get; set; }
}