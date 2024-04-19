using BeamOs.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Read;

internal class NodeReadModelConfiguration : IEntityTypeConfiguration<NodeReadModel>
{
    public void Configure(EntityTypeBuilder<NodeReadModel> builder)
    {
        builder.HasKey(n => n.Id);

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
    }
}
