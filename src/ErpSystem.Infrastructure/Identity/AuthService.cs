using ErpSystem.Application.Common;
using ErpSystem.Application.DTOs.Auth;
using ErpSystem.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ErpSystem.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName) ?? await _userManager.FindByEmailAsync(dto.UserName);
        if (user == null) return Result<AuthResponseDto>.Failure("Invalid credentials");
        if (!user.IsActive) return Result<AuthResponseDto>.Failure("User is disabled");

        var check = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!check.Succeeded) return Result<AuthResponseDto>.Failure("Invalid credentials");

        var (token, exp, roles) = await _tokenService.GenerateTokenAsync(user);
        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Token = token,
            ExpiresAt = exp,
            Roles = roles.ToList()
        });
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var existing = await _userManager.FindByNameAsync(dto.UserName);
        if (existing != null) return Result<AuthResponseDto>.Failure("Username already exists");

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = true,
            IsActive = true
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return Result<AuthResponseDto>.Failure("Registration failed", result.Errors.Select(e => e.Description).ToList());

        await _userManager.AddToRoleAsync(user, "User");

        var (token, exp, roles) = await _tokenService.GenerateTokenAsync(user);
        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email ?? string.Empty,
            Token = token,
            ExpiresAt = exp,
            Roles = roles.ToList()
        });
    }

    public async Task<Result> AssignRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure("User not found");
        if (!await _roleManager.RoleExistsAsync(role)) return Result.Failure("Role not found");
        var res = await _userManager.AddToRoleAsync(user, role);
        return res.Succeeded ? Result.Success() : Result.Failure("Failed", res.Errors.Select(e => e.Description).ToList());
    }

    public async Task<Result> CreateRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName)) return Result.Failure("Role already exists");
        var res = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        return res.Succeeded ? Result.Success() : Result.Failure("Failed", res.Errors.Select(e => e.Description).ToList());
    }

    public async Task<Result<List<UserDto>>> GetUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var list = new List<UserDto>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            list.Add(new UserDto
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName,
                Roles = roles.ToList()
            });
        }
        return Result<List<UserDto>>.Success(list);
    }
}
