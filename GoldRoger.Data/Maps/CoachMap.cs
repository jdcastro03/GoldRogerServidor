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
    public class CoachMap : IEntityTypeConfiguration<Coach>
    {
        public void Configure(EntityTypeBuilder<Coach> builder)
        {
            builder.ToTable("Coaches");

            builder.HasKey(c => c.CoachId);
            builder.Property(c => c.LicenseNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(c => c.User)
                .WithOne(u => u.Coaches)
                .HasForeignKey<Coach>(c => c.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            // Asegúrate de que la propiedad Coach en Team se mapea correctamente como una relación uno a uno
            builder.HasOne(c => c.Team)
                .WithOne(t => t.Coach)
                .HasForeignKey<Team>(t => t.CoachId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
