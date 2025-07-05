using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.NPgSqlConfiguration;

public class AccountConfiguration:IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", NpgSqlConstants.DefaultSchema);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id");
        builder.Property(x => x.ClientID).IsRequired().HasColumnName("client_id");
        builder.Property(x => x.Amount).IsRequired().HasColumnName("amount");
        
        builder.Property(x => x.IsDeleted).HasDefaultValue(false).HasColumnName("is_deleted");
        builder.HasQueryFilter(x=>x.IsDeleted==false);

        builder.HasIndex(x => x.ClientID).HasFilter("is_deleted is not null").IsUnique();

        builder.HasIndex(x => x.IsDeleted);

    }
}