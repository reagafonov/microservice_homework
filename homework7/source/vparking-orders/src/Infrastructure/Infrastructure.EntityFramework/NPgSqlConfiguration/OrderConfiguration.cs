using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.NPgSqlConfiguration;

public class OrderConfiguration: IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", NpgSqlConstants.DefaultSchema);
        builder.HasKey(nameof(Order.Id));
        builder.Property(x=>x.Id).IsRequired().HasColumnName("id");
        builder.Property(x=>x.Data).HasMaxLength(10*1024).IsRequired().HasColumnName("data");
        builder.Property(x=>x.Amount).IsRequired().HasColumnName("amount");
        builder.Property(x=>x.IsDeleted).HasDefaultValue(false).HasColumnName("is_deleted");
        builder.Property(x=>x.IsPayed).HasDefaultValue(false).HasColumnName("is_payed");
        builder.Property(x=>x.IsVerified).HasDefaultValue(false).HasColumnName("is_verified");
        builder.Property(x=>x.ClientID).IsRequired().HasColumnName("client_id");
        builder.Property(x=>x.Email).IsRequired().HasColumnName("email");
        
        builder.HasIndex(x=>new {x.ClientID, x.IsPayed,x.IsVerified}).HasDatabaseName("idx_client_id_is_payed_is_verified");
        
        builder.HasIndex(x=>x.IsDeleted).HasDatabaseName("idx_is_deleted");
        
        builder.HasQueryFilter(x=>!x.IsDeleted);
    }
}