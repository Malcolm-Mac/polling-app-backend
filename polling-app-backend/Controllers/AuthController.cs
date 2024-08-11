using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using polling_app_backend;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ITokenStore _tokenStore;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, ITokenStore tokenStore)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _tokenStore = tokenStore;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (model.password != model.confirmPassword)
        {
            return BadRequest("Passwords do not match.");
        }

        var user = new IdentityUser
        {
            UserName = model.email,
            Email = model.email
        };

        var result = await _userManager.CreateAsync(user, model.password);
        if (result.Succeeded)
        {
            return Ok("User registered successfully.");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(user.Id);

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                tokenType = "Bearer",
                accessToken = token,
                expiresIn = 3600,
                refreshToken = refreshToken
            });
        }

        return Unauthorized();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return BadRequest("User not found");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return Ok(new { ResetToken = token });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("User does not exist.");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (result.Succeeded)
        {
            return Ok("Password reset successfully.");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public IActionResult RefreshToken([FromBody] RefreshTokenDto request)
    {
        var user = _userManager.FindByIdAsync(request.UserId).Result;
        if (user == null)
        {
            return Unauthorized("Invalid user.");
        }

        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken(user.Id);

        return Ok(new
        {
            accessToken = newAccessToken,
            expiresIn = 3600,
            refreshToken = newRefreshToken
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout([FromBody] LogoutDto model)
    {
        var refreshToken = _tokenStore.GetToken(model.RefreshToken);

        if (refreshToken == null)
        {
            return BadRequest("Invalid refresh token.");
        }

        _tokenStore.RemoveToken(model.RefreshToken);

        return Ok("User logged out successfully.");
    }


    private string GenerateJwtToken(IdentityUser user)
    {
        if (user.Email == null)
        {
            throw new InvalidOperationException("User email cannot be null.");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Email)
        };

        string key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(key), "The JWT key cannot be null.");

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var symmetricKey = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static RefreshToken GenerateRefreshToken(string userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return new RefreshToken
        {
            Token = token,
            UserId = userId,
            Expiration = DateTime.UtcNow.AddDays(7)
        };
    }
}
