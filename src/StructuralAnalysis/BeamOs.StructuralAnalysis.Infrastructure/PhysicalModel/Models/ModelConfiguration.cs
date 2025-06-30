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

public class ModelEntityDeleteProposalConfiguration
    : IEntityTypeConfiguration<DeleteModelEntityProposal>
{
    public void Configure(EntityTypeBuilder<DeleteModelEntityProposal> builder)
    {
        _ = builder.HasKey(el => new
        {
            el.Id,
            el.ModelProposalId,
            el.ModelId,
        });

        builder
            .HasOne(p => p.Model)
            .WithMany()
            .HasForeignKey(p => p.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.ModelProposal)
            .WithMany(m => m.DeleteModelEntityProposals)
            .HasPrincipalKey(p => new { p.Id, p.ModelId })
            .HasForeignKey(p => new { p.ModelProposalId, p.ModelId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
