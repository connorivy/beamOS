using BeamOs.Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Identity.Api.Infrastructure;

/// <summary>
/// Build migrations from folder location
/// \beamOS\src\BeamOs.Identity\BeamOs.Identity.Api\
/// with the command
/// dotnet ef migrations add Initial --project ..\BeamOs.Identity.Infrastructure\
/// </summary>
public class BeamOsIdentityDbContext : IdentityDbContext<BeamOsUser>
{
    public BeamOsIdentityDbContext(DbContextOptions<BeamOsIdentityDbContext> options)
        : base(options) { }
}

public class IdentityDbSeeder(
    UserManager<BeamOsUser> userManager,
    IUserStore<BeamOsUser> userStore,
    RoleManager<IdentityRole> roleManager
)
{
    public async Task SeedAsync()
    {
        await this.CreateAllRoles();
        await this.CreateDefaultUsers();
    }

    private async Task CreateAllRoles()
    {
        var roles = new[] { "Admin", "Manager", "User" };
        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var roleRole = new IdentityRole(role);
            await roleManager.CreateAsync(roleRole);
        }
    }

    private async Task CreateDefaultUsers()
    {
        var defaultPassword = "Password1!";
        await this.CreateSingleUser("user@email.com", defaultPassword, ["User"]);
        await this.CreateSingleUser("manager@email.com", defaultPassword, ["Manager"]);
        await this.CreateSingleUser("admin@email.com", defaultPassword, ["Admin"]);
    }

    private async Task CreateSingleUser(string email, string password, IEnumerable<string> roles)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            return;
        }

        user = new BeamOsUser() { EmailConfirmed = true };
        await userStore.SetUserNameAsync(user, email, CancellationToken.None);

        var emailStore = (IUserEmailStore<BeamOsUser>)userStore;
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);

        var result = await userManager.CreateAsync(user, password);
        var result2 = await userManager.AddToRolesAsync(user, roles);
    }
}
