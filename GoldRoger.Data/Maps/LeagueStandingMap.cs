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
        }
    }
}