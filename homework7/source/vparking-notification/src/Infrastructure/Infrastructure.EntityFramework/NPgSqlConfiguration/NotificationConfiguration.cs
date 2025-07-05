using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.NPgSqlConfiguration;

public class NotificationConfiguration:IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", NpgSqlConstants.DefaultSchema);
        builder.HasKey(notification => notification.Id);
        builder.Property(notification => notification.Id).ValueGeneratedOnAdd().HasColumnName("id");
        builder.Property(notification => notification.ClientID).IsRequired().HasColumnName("client_id");
        builder.Property(notification => notification.MessageType).IsRequired().HasColumnName("notification_message_type");
        builder.Property(notification=>notification.Email).IsRequired().HasColumnName("email");
        builder.Property(notification => notification.OrderId).IsRequired().HasColumnName("order_id");
        
        builder.Property(notification => notification.IsDeleted).HasDefaultValue(false).HasColumnName("is_deleted");
        builder.HasQueryFilter(notification=>notification.IsDeleted==false);

        builder.HasIndex(notification => notification.OrderId).HasFilter("is_deleted is not null").IsUnique();

        builder.HasIndex(notification => notification.IsDeleted);

    }
}