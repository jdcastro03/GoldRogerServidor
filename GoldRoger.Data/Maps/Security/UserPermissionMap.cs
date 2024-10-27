using GoldRoger.Entity.Entities.Security;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Data.Maps.Security
{
    public class UserPermissionMap : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> builder)
        {
            builder.ToTable("UserPermission", "security");
            builder.HasKey(up => up.Id);

            builder.Property(up => up.Id).HasColumnName("Id");
            builder.Property(up => up.UserId).HasColumnName("UserId").IsRequired();
            builder.Property(up => up.PermissionId).HasColumnName("PermissionId").IsRequired();

            builder.HasIndex(up => up.UserId);
            builder.HasIndex(up => up.PermissionId);

            builder.HasOne(up => up.User)
                .WithMany(u => u.UserPermission)
                .HasForeignKey(up => up.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(up => up.Permission)
                .WithMany()
                .HasForeignKey(up => up.PermissionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
