using GoldRoger.Entity.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Diagnostics.Contracts;

namespace GoldRoger.Data.Maps
{
    public class LeagueStandingMap : IEntityTypeConfiguration<LeagueStanding>
    {
        public void Configure(EntityTypeBuilder<LeagueStanding> builder)
        {
            builder.ToTable("LeagueStandings");

            // Clave primaria compuesta por TournamentId y TeamId
            builder.HasKey(ls => new { ls.TournamentId, ls.TeamId });

            // Propiedades con valores predeterminados
            builder.Property(ls => ls.Points)
                .HasDefaultValue(0);

            builder.Property(ls => ls.MatchesPlayed)
                .HasDefaultValue(0);

            builder.Property(ls => ls.Wins)
                .HasDefaultValue(0);

            builder.Property(ls => ls.Draws)
                .HasDefaultValue(0);

            builder.Property(ls => ls.Losses)
                .HasDefaultValue(0);

            builder.Property(ls => ls.GoalsFor)
                .HasDefaultValue(0);

            builder.Property(ls => ls.GoalsAgainst)
                .HasDefaultValue(0);

            builder.Property(ls => ls.GoalDifference)
                .HasComputedColumnSql("GoalsFor - GoalsAgainst");

            // Relación con Team
            builder.HasOne(ls => ls.Team)
                .WithMany(t => t.LeagueStandings)
                .HasForeignKey(ls => ls.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Tournament
            builder.HasOne(ls => ls.Tournament)
                .WithMany(t => t.LeagueStandings)
                .HasForeignKey(ls => ls.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}