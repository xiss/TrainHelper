namespace TrainHelper.WebApi.Dto.Token;

public record TokenResultDto(TokenDto? Token = null, string? Error = null);