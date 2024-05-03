using BeamOs.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Read;

internal class ModelReadModelConfiguration : IEntityTypeConfiguration<ModelReadModel>
{
    public void Configure(EntityTypeBuilder<ModelReadModel> builder)
    {
        _ = builder.HasKey(x => x.Id);

        _ = builder.HasMany(m => m.Nodes).WithOne().HasForeignKey(el => el.ModelId).IsRequired();

        _ = builder
            .HasMany(m => m.Element1ds)
            .WithOne()
            .HasForeignKey(el => el.ModelId)
            .IsRequired();

        _ = builder
            .HasMany(m => m.Materials)
            .WithOne()
            .HasForeignKey(el => el.ModelId)
            .IsRequired();

        _ = builder
            .HasMany(m => m.SectionProfiles)
            .WithOne()
            .HasForeignKey(el => el.ModelId)
            .IsRequired();

        _ = builder
            .HasMany(m => m.PointLoads)
            .WithOne()
            .HasForeignKey(el => el.ModelId)
            .IsRequired();
    }
}
