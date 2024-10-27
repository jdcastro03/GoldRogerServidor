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
    public class PlayerMap : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("Players");

            builder.HasKey(p => p.PlayerId);

            builder.Property(p => p.Position)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(p => p.User)
                .WithOne(u => u.Players)
                .HasForeignKey<Player>(p => p.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación con Team, permitiendo TeamId como null
            builder.HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId)
                .IsRequired(false) // Indica que TeamId es opcional
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
