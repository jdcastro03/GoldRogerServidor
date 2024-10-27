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
    public class TeamStatsMap : IEntityTypeConfiguration<TeamStats>
    {
        public void Configure(EntityTypeBuilder<TeamStats> builder)
        {
            builder.ToTable("TeamStats");

            builder.HasKey(ts => ts.TeamId);
            builder.Property(ts => ts.GoalsFor).HasDefaultValue(0);
            builder.Property(ts => ts.GoalsAgainst).HasDefaultValue(0);

            builder.HasOne(ts => ts.Team)
                   .WithOne()
                   .HasForeignKey<TeamStats>(ts => ts.TeamId);
        }
    }
}
