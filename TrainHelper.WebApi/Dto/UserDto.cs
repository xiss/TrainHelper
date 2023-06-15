using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TrainHelper.WebApi.Dto;

public record UserDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
    public string Patronymic { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
