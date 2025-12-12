namespace MeuRh.Application.DTOs;

public record UserResponseDto(Guid Id, string Name, string Email, DateTime CreatedAt);

