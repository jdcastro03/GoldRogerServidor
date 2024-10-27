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
    public class PlayerStatsMap : IEntityTypeConfiguration<PlayerStats>
    {
        public void Configure(EntityTypeBuilder<PlayerStats> builder)
        {
            builder.ToTable("PlayerStats");

            builder.HasKey(ps => ps.PlayerId);
            builder.Property(ps => ps.Goals).HasDefaultValue(0);
            builder.Property(ps => ps.YellowCards).HasDefaultValue(0);
            builder.Property(ps => ps.RedCards).HasDefaultValue(0);

            builder.HasOne(ps => ps.Player)
                   .WithOne()
                   .HasForeignKey<PlayerStats>(ps => ps.PlayerId);
        }
    }
}
