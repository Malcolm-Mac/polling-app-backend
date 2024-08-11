namespace polling_app_backend;

public class RefreshTokenDto
{
    public required string UserId { get; set; }
    public required string RefreshToken { get; set; }
}
