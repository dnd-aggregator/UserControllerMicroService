using Microsoft.AspNetCore.Mvc;
using UserController.Application.Contracts;
using UserController.Application.Contracts.Reqests;

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
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateUserModelRequest userModel,
        CancellationToken cancellationToken)
    {
        long orderId = await _userService.RegisterUser(userModel, cancellationToken);
        return new ObjectResult(orderId) { StatusCode = StatusCodes.Status201Created };
    }
}