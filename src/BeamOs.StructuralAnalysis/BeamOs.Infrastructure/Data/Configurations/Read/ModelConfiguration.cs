using BeamOs.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Read;

internal class ModelReadModelConfiguration : IEntityTypeConfiguration<ModelReadModel>
{
    public void Configure(EntityTypeBuilder<ModelReadModel> builder)
    {
        _ = builder.HasKey(x => x.Id);

        _ = builder
            .HasMany<NodeReadModel>(m => m.Nodes)
            .WithOne()
            .HasForeignKey(node => node.ModelId)
            .IsRequired();
    }
}
