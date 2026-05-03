using ErpSystem.Application.Common;
using ErpSystem.Application.DTOs.Auth;

namespace ErpSystem.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<Result> AssignRoleAsync(string userId, string role);
    Task<Result> CreateRoleAsync(string roleName);
    Task<Result<List<UserDto>>> GetUsersAsync();
}
