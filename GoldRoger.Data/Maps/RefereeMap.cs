using GoldRoger.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRoger.Data.Maps
{
    public class RefereeMap : IEntityTypeConfiguration<Referee>
    {
        public void Configure(EntityTypeBuilder<Referee> builder)
        {
            builder.ToTable("Referees");

            builder.HasKey(r => r.RefereeId);
            builder.Property(r => r.LicenseNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(r => r.User)
                .WithOne(u => u.Referees)
                .HasForeignKey<Referee>(r => r.RefereeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
