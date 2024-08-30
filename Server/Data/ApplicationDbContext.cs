using CarChecker.Server.Models;
using CarChecker.Shared;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CarChecker.Server.Data
{
    public class ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : ApiAuthorizationDbContext<ApplicationUser>(options, operationalStoreOptions)
    {
        public DbSet<Vehicle> Vehicles { get; set; }
    }
}
