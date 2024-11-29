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
    public class TeamMap : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("Teams");

            builder.HasKey(t => t.TeamId);
            builder.Property(t => t.TeamName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(t => t.Coach)
                .WithOne(c => c.Team)
                .HasForeignKey<Team>(t => t.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<LeagueStanding>()
    .WithOne()
    .HasForeignKey(ls => ls.TeamId)
    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
