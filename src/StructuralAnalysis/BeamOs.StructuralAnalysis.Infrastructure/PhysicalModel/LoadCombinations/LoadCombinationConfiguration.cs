using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.LoadCombinations;

public class LoadCombinationConfiguration : IEntityTypeConfiguration<LoadCombination>
{
    public void Configure(EntityTypeBuilder<LoadCombination> builder)
    {
        _ = builder
            .HasOne(el => el.Model)
            .WithMany(el => el.LoadCombinations)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
