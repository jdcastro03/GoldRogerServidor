using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldRoger.Entity.Entities;

namespace GoldRoger.Data.Maps
{
    public class MatchMap : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            builder.ToTable("Matches");

            builder.HasKey(m => m.MatchId);
            builder.Property(m => m.Score)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(m => m.Tournament)
                .WithMany(t => t.Matches)
                .HasForeignKey(m => m.TournamentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Team1)
                .WithMany()
                .HasForeignKey(m => m.Team1Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Team2)
                .WithMany()
                .HasForeignKey(m => m.Team2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
