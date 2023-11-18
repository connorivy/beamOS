using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using Microsoft.EntityFrameworkCore;
using BeamOS.Common.Infrastructure;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Infrastructure.Common.Configurations;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using Microsoft.Extensions.DependencyInjection;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Infrastructure;

public class PhysicalModelDbContext : DbContext
{
    public PhysicalModelDbContext(DbContextOptions<PhysicalModelDbContext> options) : base(options) { }

    public DbSet<Model> Models { get; set; }
    public DbSet<Element1D> Element1Ds { get; set; }
    public DbSet<Node> Nodes { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddCommonInfrastructure();
        configurationBuilder.AddPhysicalModelInfrastructure();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .ApplyConfigurationsFromAssembly(typeof(NodeConfiguration).Assembly);

        builder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.IsPrimaryKey())
            .ToList()
            .ForEach(p => p.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never);
    }

    public static async Task SeedSqlServer(IServiceProvider applicationServices)
    {
        using var scope = applicationServices.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<PhysicalModelDbContext>();
        dbContext.Database.EnsureCreated();

        ModelId zeroId = new(Guid.Parse("00000000-0000-0000-0000-000000000000"));
        if (await dbContext.Models
            .AnyAsync(m => m.Id == zeroId))
        {
            return;
        }

        dbContext.Models.Add(new(
            "Big Ol' Building",
            "Building on the corner of 6th and Main",
            new(UnitSettings.K_IN),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        ));

        dbContext.Nodes.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            0,
            0,
            0,
            UnitSettings.K_IN.LengthUnit,
            new(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        ));

        dbContext.Nodes.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            0,
            0,
            10,
            UnitSettings.K_IN.LengthUnit,
            new(Guid.Parse("00000000-0000-0000-0000-000000000001"))
        ));

        dbContext.Nodes.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            0,
            0,
            -10,
            UnitSettings.K_IN.LengthUnit,
            new(Guid.Parse("00000000-0000-0000-0000-000000000002"))
        ));

        dbContext.Element1Ds.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000001")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        ));

        dbContext.Element1Ds.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000001")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000002")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        ));

        await dbContext.SaveChangesAsync();
    }
}
