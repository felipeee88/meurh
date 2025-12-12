namespace MeuRh.Api.Models;

public class ApiResponse<T>
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string message = "Operação realizada com sucesso")
    {
        return new ApiResponse<T>
        {
            Status = "Sucesso",
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Error(string message)
    {
        return new ApiResponse<T>
        {
            Status = "Erro",
            Message = message,
            Data = default
        };
    }
}

