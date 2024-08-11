namespace polling_app_backend;

public class RegisterDto
{
    public required string firstName { get; set; }
    public required string lastName { get; set; }
    public required string email { get; set; }
    public required string password { get; set; }
    public required string confirmPassword { get; set; }

}
