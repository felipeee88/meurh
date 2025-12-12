using MediatR;
using Microsoft.AspNetCore.Mvc;
using MeuRh.Api.Models;
using MeuRh.Api.Models.Requests;
using MeuRh.Application.Commands.Login;
using MeuRh.Application.Commands.RegisterUser;
using MeuRh.Application.DTOs;

namespace MeuRh.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse<LoginResponseDto>.Success(result, "Login realizado com sucesso");
        return Ok(response);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Name, request.Email, request.Password);
        await _mediator.Send(command, cancellationToken);
        var response = ApiResponse<object>.Success(default!, "Usuário cadastrado com sucesso");
        return StatusCode(StatusCodes.Status201Created, response);
    }
}

