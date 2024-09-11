using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Infrastructure.Data.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations;

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
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        _ = builder
            .HasOne(el => el.SectionProfile)
            .WithMany()
            .HasForeignKey(el => el.SectionProfileId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        //builder.OwnsOne(el => el.CustomData);
        //.HasConversion<string>(new DictStringObjConverter())
        //.HasColumnType("nvarchar(128)")

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
