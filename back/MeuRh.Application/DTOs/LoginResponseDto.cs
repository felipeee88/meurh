namespace MeuRh.Application.DTOs;

public record LoginResponseDto(string AccessToken, string TokenType, int ExpiresIn);

