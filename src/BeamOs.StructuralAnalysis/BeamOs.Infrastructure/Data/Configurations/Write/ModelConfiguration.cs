using BeamOs.Domain.AnalyticalResults.ModelResultAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Write;

internal class ModelConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        _ = builder
            .HasMany(m => m.Nodes)
            .WithOne()
            .HasForeignKey(node => node.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasMany(m => m.Element1ds)
            .WithOne()
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasMany(m => m.Materials)
            .WithOne()
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasMany(m => m.SectionProfiles)
            .WithOne()
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(m => m.ModelResults)
            .WithOne()
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // these belong to the node
        //_ = builder.HasMany<PointLoad>().WithOne().HasForeignKey(el => el.ModelId).IsRequired();
        //_ = builder.HasMany<MomentLoad>().WithOne().HasForeignKey(el => el.ModelId).IsRequired();
    }
}
