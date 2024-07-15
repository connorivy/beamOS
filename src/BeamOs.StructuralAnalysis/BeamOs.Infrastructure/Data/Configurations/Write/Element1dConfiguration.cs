using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Write;

public class Element1dConfiguration : IEntityTypeConfiguration<Element1D>
{
    public void Configure(EntityTypeBuilder<Element1D> builder)
    {
        //_ = builder
        //    .HasOne(el => el.StartNode)
        //    .WithOne()
        //    .OnDelete(DeleteBehavior.NoAction);

        //_ = builder
        //    .HasOne(el => el.EndNode)
        //    .WithOne()
        //    .OnDelete(DeleteBehavior.NoAction);

        _ = builder
            .HasOne(el => el.Material)
            .WithMany()
            .HasForeignKey(el => el.MaterialId)
            .OnDelete(DeleteBehavior.ClientCascade);

        _ = builder
            .HasOne(el => el.SectionProfile)
            .WithMany()
            .HasForeignKey(el => el.SectionProfileId)
            .OnDelete(DeleteBehavior.ClientCascade);

        //_ = builder
        //    .HasOne(el => el.SectionProfile)
        //    .OnDelete(DeleteBehavior.NoAction);

        //_ = builder
        //    .HasOne<Model>()
        //    .WithMany(m => m.Element1Ds)
        //    .HasForeignKey(el => el.ModelId)
        //    .OnDelete(DeleteBehavior.NoAction);
    }
}
