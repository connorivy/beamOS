using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Octrees;

// DTO for serializing NodeIdAndLocation
internal record NodeLocationDto(int NodeId, double X, double Y, double Z);

internal class OctreeConfiguration : IEntityTypeConfiguration<Octree>
{
    public void Configure(EntityTypeBuilder<Octree> builder)
    {
        builder
            .HasOne(o => o.Root)
            .WithOne()
            .HasForeignKey<Octree>(o => o.RootId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class OctreeNodeConfiguration : IEntityTypeConfiguration<OctreeNode>
{
    public void Configure(EntityTypeBuilder<OctreeNode> builder)
    {
        // Configure the self-referencing relationship for children
        builder
            .HasMany(n => n.Children)
            .WithOne()
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // Store the Objects list as JSON with a custom serialization strategy
        // We serialize NodeIdAndLocation as a simple DTO (NodeId, X, Y, Z)
        builder.Property(n => n.Objects)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(
                    v.Select(item => new NodeLocationDto( 
                        item.NodeId.Id, 
                        item.LocationPoint.X.Meters,
                        item.LocationPoint.Y.Meters,
                        item.LocationPoint.Z.Meters
                    )).ToList()
                ),
                v => (System.Text.Json.JsonSerializer.Deserialize<List<NodeLocationDto>>(v) ?? new List<NodeLocationDto>())
                    .Select(dto => new NodeIdAndLocation(
                        new NodeId(dto.NodeId),
                        new Point(dto.X, dto.Y, dto.Z, LengthUnit.Meter)
                    )).ToList()
            )
            .HasColumnType("jsonb");

        // Configure the complex type for Center (Point)
        builder.ComplexProperty(n => n.Center, centerBuilder =>
        {
            centerBuilder.Property(p => p.X);
            centerBuilder.Property(p => p.Y);
            centerBuilder.Property(p => p.Z);
        });
    }
}
