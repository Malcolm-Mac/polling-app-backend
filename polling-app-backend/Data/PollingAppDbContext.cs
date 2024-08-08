using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace polling_app_backend.Data;

public class PollingAppDbContext : IdentityDbContext<IdentityUser>
{
    public PollingAppDbContext(DbContextOptions options) : base(options){}

}
