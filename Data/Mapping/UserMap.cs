using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCoreExampleApi.Data.Entities;

namespace NetCoreExampleApi.Data.Mapping;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(p => p.Id);
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Property(p => p.Name).HasColumnType("nvarchar(256)");
        builder.Property(p => p.Surename).HasColumnType("nvarchar(256)");
        builder.Property(p => p.IsDeleted).HasColumnType("BIT");
    }
}