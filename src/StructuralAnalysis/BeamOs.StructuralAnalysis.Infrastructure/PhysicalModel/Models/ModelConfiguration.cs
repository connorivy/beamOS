using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

internal class ModelConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        _ = builder
            .HasMany(m => m.Element1ds)
            .WithOne(el => el.Model)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasMany(m => m.Materials)
            .WithOne(el => el.Model)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasMany(m => m.SectionProfiles)
            .WithOne(el => el.Model)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder
            .HasMany(m => m.ResultSets)
            .WithOne(el => el.Model)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasOne(m => m.AnalyticalResults)
        //    .WithOne()
        //    .HasForeignKey<AnalyticalResults>(el => el.ModelId)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(m => m.PointLoads)
            .WithOne(el => el.Model)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(m => m.MomentLoads)
            .WithOne(el => el.Model)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        //builder
        //    .HasMany(m => m.MomentLoads)
        //    .WithOne()
        //    .HasForeignKey(el => el.ModelId)
        //    .IsRequired()
        //    .OnDelete(DeleteBehavior.Cascade);

        // these belong to the node
        //_ = builder.HasMany<PointLoad>().WithOne().HasForeignKey(el => el.ModelId).IsRequired();
        //_ = builder.HasMany<MomentLoad>().WithOne().HasForeignKey(el => el.ModelId).IsRequired();
    }
}
