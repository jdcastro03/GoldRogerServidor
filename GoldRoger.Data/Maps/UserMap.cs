using GoldRoger.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldRoger.Entity.Entities.GoldRoger.Entity.Entities;

namespace GoldRoger.Data.Maps
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.UserId);
            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(u => u.UserType)
                .IsRequired();

            builder.HasOne(u => u.UserTypeNavigation)
                .WithMany(ut => ut.Users)
                .HasForeignKey(u => u.UserType)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
