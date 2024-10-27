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
    public class TournamentMap : IEntityTypeConfiguration<Tournament>
    {
        public void Configure(EntityTypeBuilder<Tournament> builder)
        {
            builder.ToTable("Tournaments");

            builder.HasKey(t => t.TournamentId);
            builder.Property(t => t.TournamentName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(t => t.StartDate)
                .IsRequired();
            builder.Property(t => t.EndDate)
                .IsRequired();

            builder.HasOne(t => t.Organizer)
                .WithMany(o => o.Tournaments)
                .HasForeignKey(t => t.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.TournamentType)
                .WithMany(tt => tt.Tournaments)
                .HasForeignKey(t => t.TournamentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
