﻿namespace polling_app_backend;

public class RefreshToken
{
    public required string Token { get; set; }
    public required string UserId { get; set; }
    public DateTime Expiration { get; set; }
}

