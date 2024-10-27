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
    public class OrganizerMap : IEntityTypeConfiguration<Organizer>
    {
        public void Configure(EntityTypeBuilder<Organizer> builder)
        {
            builder.ToTable("Organizers");

            builder.HasKey(o => o.OrganizerId);
            builder.Property(o => o.OrganizationName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(o => o.User)
                .WithOne(u => u.Organizers)
                .HasForeignKey<Organizer>(o => o.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
