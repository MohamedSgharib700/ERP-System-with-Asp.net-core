using ErpSystem.Application.DTOs.Auth;
using ErpSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _auth.LoginAsync(dto);
        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _auth.RegisterAsync(dto);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Users() => Ok(await _auth.GetUsersAsync());

    [HttpPost("roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        var r = await _auth.CreateRoleAsync(roleName);
        return r.Succeeded ? Ok(r) : BadRequest(r);
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole(AssignRoleDto dto)
    {
        var r = await _auth.AssignRoleAsync(dto.UserId, dto.Role);
        return r.Succeeded ? Ok(r) : BadRequest(r);
    }
}
