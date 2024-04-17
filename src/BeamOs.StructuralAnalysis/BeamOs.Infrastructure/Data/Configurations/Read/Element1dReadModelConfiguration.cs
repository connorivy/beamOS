using BeamOs.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeamOs.Infrastructure.Data.Configurations.Read;

internal class Element1dReadModelConfiguration : IEntityTypeConfiguration<Element1dReadModel>
{
    public void Configure(EntityTypeBuilder<Element1dReadModel> builder)
    {
        builder.HasKey(el => el.Id);

        builder.HasOne(el => el.StartNode).WithMany().HasForeignKey(el => el.StartNodeId);

        builder.HasOne(el => el.EndNode).WithMany().HasForeignKey(el => el.EndNodeId);
    }
}
