using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeuRh.Api.Models;
using MeuRh.Api.Models.Requests;
using MeuRh.Application.Commands.CreateUser;
using MeuRh.Application.Commands.DeleteUser;
using MeuRh.Application.DTOs;
using MeuRh.Application.Queries.GetUsers;

namespace MeuRh.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(request.Name, request.Email, request.Password);
        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse<UserResponseDto>.Success(result, "Usuário cadastrado com sucesso");
        return CreatedAtAction(nameof(GetUsers), new { id = result.Id }, response);
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<UserResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<List<UserResponseDto>>>> GetUsers(
        [FromQuery] string? name,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery(name);
        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<List<UserResponseDto>>.Success(result, "Usuários listados com sucesso");
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result)
        {
            var errorResponse = ApiResponse<object>.Error("Usuário não encontrado");
            return NotFound(errorResponse);
        }

        var successResponse = ApiResponse<object>.Success(default!, "Usuário excluído com sucesso");
        return Ok(successResponse);
    }
}

