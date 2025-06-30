using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.SectionProfiles;

public class SectionProfileInfoBaseConfiguration : IEntityTypeConfiguration<SectionProfileInfoBase>
{
    public void Configure(EntityTypeBuilder<SectionProfileInfoBase> builder)
    {
        builder
            .UseTphMappingStrategy()
            .HasDiscriminator(el => el.SectionProfileType)
            .HasValue<SectionProfileFromLibrary>(BeamOsObjectType.SectionProfileFromLibrary)
            .HasValue<SectionProfile>(BeamOsObjectType.SectionProfile);

        builder
            .Property(el => el.SectionProfileType)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
    }
}

public class SectionProfileConfiguration : IEntityTypeConfiguration<SectionProfile>
{
    public void Configure(EntityTypeBuilder<SectionProfile> builder)
    {
        builder
            .HasOne(el => el.Model)
            .WithMany(el => el.SectionProfiles)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SectionProfileFromLibraryConfiguration
    : IEntityTypeConfiguration<SectionProfileFromLibrary>
{
    public void Configure(EntityTypeBuilder<SectionProfileFromLibrary> builder)
    {
        builder
            .HasOne(el => el.Model)
            .WithMany(el => el.SectionProfilesFromLibrary)
            .HasForeignKey(el => el.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
