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
    public class PermissionMap : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permission", "security");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnName("Id");
            builder.Property(p => p.Key).HasColumnName("Key").IsRequired();
            builder.Property(p => p.Description).HasColumnName("Description").HasMaxLength(255);
            builder.Property(p => p.CreatedBy).HasColumnName("CreatedBy").HasMaxLength(100);
            builder.Property(p => p.CreatedOn).HasColumnName("CreatedOn").HasColumnType("datetime").IsRequired();
            builder.Property(p => p.ModifiedBy).HasColumnName("ModifiedBy").HasMaxLength(100);
            builder.Property(p => p.ModifiedOn).HasColumnName("ModifiedOn").HasColumnType("datetime");
        }
    }
}
