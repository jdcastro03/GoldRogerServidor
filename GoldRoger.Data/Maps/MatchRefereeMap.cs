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
    public class MatchRefereeMap : IEntityTypeConfiguration<MatchReferee>
    {
        public void Configure(EntityTypeBuilder<MatchReferee> builder)
        {
            builder.ToTable("MatchReferees");

            builder.HasKey(mr => new { mr.MatchId, mr.RefereeId });

            builder.HasOne(mr => mr.Match)
                .WithMany(m => m.MatchReferees)
                .HasForeignKey(mr => mr.MatchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mr => mr.Referee)
                .WithMany(r => r.MatchReferees)
                .HasForeignKey(mr => mr.RefereeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
