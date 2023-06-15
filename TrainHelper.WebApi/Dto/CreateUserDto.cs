using System.ComponentModel.DataAnnotations;

namespace TrainHelper.WebApi.Dto;

public record CreateUserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    [Required]
    [Compare(nameof(PasswordRetry))]
    public string Password { get; init; } = string.Empty;

    [Required]
    public string PasswordRetry { get; init; } = string.Empty;
    public string Patronymic { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
}