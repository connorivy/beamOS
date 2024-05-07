using BeamOs.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Read;

internal class NodeReadModelConfiguration : IEntityTypeConfiguration<NodeReadModel>
{
    public void Configure(EntityTypeBuilder<NodeReadModel> builder)
    {
        builder.HasKey(n => n.Id);

        builder.ComplexProperty(n => n.Restraint);

        _ = builder
            .HasMany(n => n.PointLoads)
            .WithOne(m => m.Node)
            .HasForeignKey(el => el.NodeId)
            .IsRequired();

        _ = builder
            .HasMany(n => n.MomentLoads)
            .WithOne(m => m.Node)
            .HasForeignKey(el => el.NodeId)
            .IsRequired();

        _ = builder
            .HasOne(n => n.NodeResult)
            .WithOne(m => m.Node)
            .HasForeignKey<NodeResultReadModel>(el => el.NodeId)
            .IsRequired();
    }
}

internal class NodeReadModelConfiguration2 : IEntityTypeConfiguration<PointLoadReadModel>
{
    public void Configure(EntityTypeBuilder<PointLoadReadModel> builder)
    {
        builder.HasKey(n => n.Id);

        //_ = builder
        //    .HasOne(n => n.Model)
        //    .WithMany(m => m.PointLoads)
        //    .HasForeignKey(n => n.Node.ModelId);

        //    .WithOne(m => m.Node)
        //    .HasForeignKey(el => el.NodeId)
        //    .IsRequired();

        //_ = builder
        //    .HasMany(n => n.MomentLoads)
        //    .WithOne(m => m.Node)
        //    .HasForeignKey(el => el.NodeId)
        //    .IsRequired();
    }
}
