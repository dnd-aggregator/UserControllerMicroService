using Microsoft.AspNetCore.Mvc;
using UserController.Application.Contracts;
using UserController.Application.Contracts.Reqests;
using UserController.Application.Models;

namespace UserController.Presentation.Http.Controllers;

[ApiController]
[Route("products/test/[controller]")]
public class UserPlayerController : ControllerBase
{
    private readonly IUserService _userService;

    public UserPlayerController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Создает новый продукт.
    /// </summary>
    /// /// <param name="userModel">Модель для создания пользователя.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат выполнения операции создания продукта.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserModelRequest userModel,
        CancellationToken cancellationToken)
    {
        long orderId = await _userService.RegisterUser(userModel, cancellationToken);
        return new ObjectResult(orderId) { StatusCode = StatusCodes.Status201Created };
    }

    /// <summary>
    /// Создает новый продукт.
    /// </summary>
    /// /// <param name="userId">Модель для создания пользователя.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат выполнения операции создания продукта.</returns>
    [HttpGet]
    public async Task<IActionResult> GetUserById(
        [FromQuery] long userId,
        CancellationToken cancellationToken)
    {
        UserModel? model = await _userService.GetUser(userId, cancellationToken);
        if (model == null)
        {
            return NotFound();
        }

        return new ObjectResult(model) { StatusCode = StatusCodes.Status200OK };
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser(
        [FromBody] UserModel userModel,
        CancellationToken cancellationToken)
    {
        await _userService.UpdateUser(userModel, cancellationToken);
        return new OkResult();
    }
}